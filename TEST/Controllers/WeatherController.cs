using GetWeatherInfo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CurrentWeather.Models;
using System.Web;
using CurrentWeather.Core;

namespace CurrentWeather.Controllers
{
    /// <summary>
    /// Weather controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;

        private readonly AppConfig _config;

        private readonly HttpClient _client;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="client"></param>
        public WeatherController(ILogger<WeatherController> logger, AppConfig config, HttpClient client)
        {
            _logger = logger;
            _config = config;
            _client = client;
        }

        /// <summary>
        /// Get weather by city and state
        /// Retrieves weather information from openweathermap API 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <returns>Current temperature, air speed, and pressure</returns>
        [HttpGet("bycity")]
        public async Task<TempSpeedPressure> GetAsync([Required][FromQuery]string city, [Required][FromQuery]string state)
        {

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={HttpUtility.UrlEncode(city)},{HttpUtility.UrlEncode(state)},us&appid={_config.id}";

            _client.BaseAddress = new Uri(url);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            WeatherInfo wi = null;
            TempSpeedPressure retValues = new TempSpeedPressure();

            HttpResponseMessage response = await _client.GetAsync(string.Empty);
            if(response.IsSuccessStatusCode)
            {
                wi = await response.Content.ReadAsAsync<WeatherInfo>();
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new HttpException(response.StatusCode, "City not found");
            }

            retValues.temp = "Temperature (F): " + ((wi.main.temp - 273.15) * 9 / 5 + 32);
            retValues.feels_like = "Feels like (F): " + ((wi.main.feels_like - 273.15) * 9 / 5 + 32);
            retValues.speed = "Wind speed (m/s): " + wi.wind.speed;
            retValues.pressure = "Pressure (hPa): " + wi.main.pressure;

            return retValues;

        }

        

    }
}
