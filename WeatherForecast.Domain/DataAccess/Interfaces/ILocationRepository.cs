using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.DataAccess.Interfaces
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetLocationsAsync();
        Task<Domain.Entities.Location> GetMostRecentAsync();
        Task<Domain.Entities.Location?> GetLocationAsyncById(int id);
        Task<int?> GetLocationIdByLatitudeLongiture(double latitude, double longitude);
        Task<int> InsertLocationAsync(Location location);
        Task<int> UpdateLocationAsync(Location location);
        Task DeleteLocationAsyncById(int id);
    }
}
