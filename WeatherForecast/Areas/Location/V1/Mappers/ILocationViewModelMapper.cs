using WeatherForecast.Areas.Location.V1.ViewModels;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Areas.Location.V1.Mappers
{
    public interface ILocationViewModelMapper
    {
        Domain.Entities.Location Map(LocationInsertRequestModel locationInsertRequestModel);
        Domain.Entities.Location Map(LocationUpdateRequestModel locationUpdateRequestModel);
        LocationViewModel Map(Domain.Entities.Location location);
        List<LocationViewModel> Map(List<Domain.Entities.Location> locations);
    }
}
