using Microsoft.Azure.Amqp.Framing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Models
{
    internal class WeatherResponse
    {
        private int _temperature;
        public int Temperature
        {
            get => _temperature;
            set
            {
                _temperature = (int)(value - 273.15);
            }
        }
        public int Humidity { get; set; }
    }
}
