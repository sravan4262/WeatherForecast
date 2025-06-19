using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Entities
{
    //todo: make properties get, set
    public class Location
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime AccessedDateTime { get; set; }

        public virtual List<WeatherForecast> WeatherForecasts { get; set; }
    }
}
