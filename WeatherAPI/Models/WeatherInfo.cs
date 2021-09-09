using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrentWeather.Models
{
    /// <summary>
    /// Model for openweathermap API
    /// </summary>
    public class WeatherInfo
    {
        public GPS coord { get; set; }
        public Weather[] weather { get; set; }
        public string Base { get; set; }
        public Temperatureinfo main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public long dt { get; set; }
        public Sys sys { get; set; }
        public long timezone { get; set; }
        public long id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }

    }
    
}