using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Areas.Location.V1.ViewModels;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.WebApi.UnitTests.V1.Helpers
{
    public static class LocationGenerator
    {
        public static Location DefaultLocation =>
            new Location
            {
                Id = 1,
                Latitude = 34.0522,
                Longitude = -118.2437,
                AccessedDateTime = DateTime.UtcNow
            };

        public static List<Location> DefaultLocationList =>
            new List<Location>
            {
                new Location
                {
                    Id = 1, Latitude = 34.0522, Longitude = -118.2437, AccessedDateTime = DateTime.UtcNow
                },
                new Location
                {
                    Id = 2, Latitude = 40.7128, Longitude = -74.0060, AccessedDateTime = DateTime.UtcNow
                }
            };

        public static LocationInsertRequestModel DefaultInsertRequestModel =>
            new LocationInsertRequestModel
            {
                Latitude = 30.2672,
                Longitude = -97.7431                
            };

        public static LocationUpdateRequestModel DefaultUpdateRequestModel =>
            new LocationUpdateRequestModel
            {
                Id = 1,
                Latitude = 34.0522,
                Longitude = -118.2437
            };
    }

    public static class LocationViewModelGenerator
    {
        public static LocationViewModel DefaultLocationViewModel =>
            new LocationViewModel
            {
                Id = 1,
                Latitude = 34.0522,
                Longitude = -118.2437,
                AccessedDateTime = DateTime.UtcNow
            };

        public static List<LocationViewModel> DefaultLocationViewModelList =>
            new List<LocationViewModel>
            {
                new LocationViewModel
                {
                    Id = 1, Latitude = 34.0522, Longitude = -118.2437, AccessedDateTime = DateTime.UtcNow
                },
                new LocationViewModel
                {
                    Id = 2, Latitude = 40.7128, Longitude = -74.0060, AccessedDateTime = DateTime.UtcNow
                }
            };
    }
}
