using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CurrentWeather.Core
{
    /// <summary>
    /// Global logging class
    /// Intercepts incoming requests and outgoing responses
    /// </summary>
    public class LoggingMiddleware
    {
       
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoggingMiddleware> _logger;
        public LoggingMiddleware(RequestDelegate next, 
            IConfiguration configuration,
            ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }
        /// <summary>
        /// Invokes the request and handles exceptions
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Guid id = Guid.NewGuid();
            //First, get the incoming request
            var request = await FormatRequest(context.Request);
            //Logger.Info(() => { return $"REQUEST MESSAGE: {request}"; }, false, context.Request.Path, context.Request.Method, context.User?.Identity?.Name ?? string.Empty);
            _logger.LogInformation($"Incoming request", context.Request.Path, context.Request.Method, id);
            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;
            
            //Create a new memory stream...
            using (var responseStream = new MemoryStream())
            {
                //...and use that for the temporary response body
                context.Response.Body = responseStream;

                try
                {
                    //Continue down the Middleware pipeline, eventually returning to this class
                    await _next(context);
                }
                catch (Exception e)
                {
                    
                    context.Response.ContentType = "application/json";
                    if (e is HttpException)
                    {
                        _logger.LogWarning(e.ToString());
                        var ex = e as HttpException;
                        context.Response.StatusCode = (int)((HttpException)e).HttpStatus;
                        await context.Response.WriteAsync(ex.Error.ToJson());
                    }
                    else
                    {
                        _logger.LogError(e.ToString());
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await context.Response.WriteAsync(new ErrorEntity()
                        {
                            Status = (int)HttpStatusCode.InternalServerError,
                            Title = "System error",
                        }.ToJson()); 
                    }
                }
                //Format the response from the server
                var response = await FormatResponse(context.Response, watch);
                _logger.LogInformation($"{id} - outgoing response: {response}");
                //Save log to chosen datastore
                watch.Stop();

                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseStream.CopyToAsync(originalBodyStream);
            }

        }
        /// <summary>
        /// Gets formatted request information
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<string> FormatRequest(HttpRequest request)
        {

            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body.Seek(0, SeekOrigin.Begin);

            return $"{request.Scheme} {request.Host}{request.Path} QUERIES: {request.QueryString}; MESSAGE BODY: {bodyAsText}; REQUEST HEADERS: {request.Headers.ToJson()}";
        }
        /// <summary>
        /// Gets formatted response information
        /// </summary>
        /// <param name="response"></param>
        /// <param name="watch"></param>
        /// <returns></returns>
        private async Task<string> FormatResponse(HttpResponse response, Stopwatch watch)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            watch.Stop();
            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"EXECUTION TIME: {watch.ElapsedMilliseconds}; RESPONSE: {response.StatusCode} {text}; {response.Cookies.ToJson()}";
        }
    }

}
