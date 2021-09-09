using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrentWeather.Models
{
    /// <summary>
    /// Model for openweathermap API
    /// </summary>
    public class TempSpeedPressure
    {
        public string temp { get; set; }
        public string feels_like { get; set; }
        public string speed { get; set; }
        public string pressure { get; set; }

    }
}
