using WeatherForecast.Areas.Location.V1.ViewModels;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Areas.Location.V1.Mappers
{
    public class LocationViewModelMapper : ILocationViewModelMapper
    {
        public Domain.Entities.Location Map(LocationInsertRequestModel locationInsertRequestModel)
        {
            return new Domain.Entities.Location()
            {
                Latitude = locationInsertRequestModel.Latitude,
                Longitude = locationInsertRequestModel.Longitude,
                AccessedDateTime = DateTime.UtcNow
            };
        }

        public Domain.Entities.Location Map(LocationUpdateRequestModel locationUpdateRequestModel)
        {
            return new Domain.Entities.Location()
            {
                Id = locationUpdateRequestModel.Id,
                Latitude = locationUpdateRequestModel.Latitude,
                Longitude = locationUpdateRequestModel.Longitude,
                AccessedDateTime = DateTime.UtcNow
            };
        }

        public LocationViewModel Map(Domain.Entities.Location location)
        {
            return new LocationViewModel()
            {
                Id = location.Id,
                Longitude = location.Longitude,
                Latitude = location.Latitude,
                AccessedDateTime = location.AccessedDateTime
            };
        }

        public List<LocationViewModel> Map(List<Domain.Entities.Location> locations)
        {
            var viewModels = new List<LocationViewModel>();
            foreach (var location in locations)
            {
                viewModels.Add(Map(location));
            }
            return viewModels;
        }
    }
}
