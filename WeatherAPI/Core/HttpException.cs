using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CurrentWeather.Core
{
    
    [DataContract]
    public class ErrorEntity
    {
        public int Status { get; set; }

        public string Title { get; set; }

        public Dictionary<string, IEnumerable<string>> Errors { get; set; }
    }
    /// <summary>
    /// Http request exception for errors such as not found, unauthorized
    /// </summary>
    [Serializable]
    public class HttpException : Exception
    {
        public ErrorEntity Error { get; set; }
        public HttpStatusCode HttpStatus { get; set; }

        public HttpException(HttpStatusCode HttpStatus, string Message = null,  Dictionary<string, IEnumerable<string>> details=null, Exception InnerException = null)
            : base(Message, InnerException)
        {
            Error = new ErrorEntity()
            {
                Title = Message,
                Status = (int)HttpStatus,
                Errors = details
            };
            this.HttpStatus = HttpStatus;
        }

    }

}
