using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.Business.Interfaces
{
    public interface ILocationRetriever
    {
        Task<List<Location>> GetLocationsAsync();
        Task<Location> GetMostRecentAsync();
        Task<Location?> GetLocationAsyncById(int id);
    }
}
