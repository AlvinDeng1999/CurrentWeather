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

namespace CurrentWeather.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetWeatherInfo : ControllerBase
    {
        private readonly ILogger<GetWeatherInfo> _logger;

        private readonly AppData _config;

        private readonly HttpClient _client;
        public GetWeatherInfo(ILogger<GetWeatherInfo> logger, AppData config, HttpClient client)
        {
            _logger = logger;
            _config = config;
            _client = client;
        }

        [HttpGet("bycity")]
        public async Task<TempSpeedPressure> GetAsync([Required][FromQuery]string city, [Required][FromQuery]string state)
        {

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city},{state},us&appid={_config.id}";
            
            _client.BaseAddress = new Uri(url);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            WeatherInfo wi = null;
            HttpResponseMessage response = await _client.GetAsync(string.Empty);
            if(response.IsSuccessStatusCode)
            {
                wi = await response.Content.ReadAsAsync<WeatherInfo>();
            }

            TempSpeedPressure retValues = new TempSpeedPressure();
            retValues.temp = "Temperature (F): " + ((wi.main.temp - 273.15) * 9 / 5 + 32);
            retValues.feels_like = "Feels like (F): " + ((wi.main.feels_like - 273.15) * 9 / 5 + 32);
            retValues.speed = "Wind speed (m/s): " + wi.wind.speed;
            retValues.pressure = "Pressure (hPa): " + wi.main.pressure;

            return retValues;

        }
        
        

    }
}
