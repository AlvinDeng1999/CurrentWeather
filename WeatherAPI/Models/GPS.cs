using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrentWeather.Models
{
    /// <summary>
    /// Model for openweathermap API
    /// </summary>
    public class GPS
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }
}
