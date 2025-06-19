using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Business.Interfaces;
using WeatherForecast.Domain.DataAccess.Interfaces;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.Business.Classes
{
    public class LocationRetriever : ILocationRetriever
    {
        private readonly ILocationRepository _locationRepository;
        public LocationRetriever(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public Task<Location?> GetLocationAsyncById(int id)
        {
            if(id < 0)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var location = _locationRepository.GetLocationAsyncById(id);
            return location;
        }

        public async Task<List<Location>> GetLocationsAsync()
        {
            return await _locationRepository.GetLocationsAsync();
        }

        public async Task<Location> GetMostRecentAsync()
        {
            return await _locationRepository.GetMostRecentAsync();
        }
    }
}
