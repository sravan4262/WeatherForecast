using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Entities
{
    public class WeatherForecast
    {
        public int Id { get; set; }        
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public int WeatherCode { get; set; }
        public string Timezone { get; set; }
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }
    }
}
