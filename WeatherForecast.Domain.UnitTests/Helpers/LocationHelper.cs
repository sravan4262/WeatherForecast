using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.UnitTests.Helpers
{
    public static class LocationHelper
    {
        public static Location Default()
        {
            return new Location()
            {
                Id = 1,
                Latitude = 1,
                Longitude = 1,
                AccessedDateTime = DateTime.Now
            };
        }

        public static List<Location> DefaultList()
        {
            return new List<Location>()
            {
                new Location()
                {
                Id = 1,
                Latitude = 1,
                Longitude = 1,
                AccessedDateTime = DateTime.Now
                },
                new Location()
                {
                    Id = 2,
                Latitude = 2,
                Longitude = 2,
                AccessedDateTime = DateTime.Now.AddDays(1)
                }
            };
        }
    }
}
