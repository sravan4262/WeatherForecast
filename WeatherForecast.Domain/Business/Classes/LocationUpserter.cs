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
    public class LocationUpserter : ILocationUpserter
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ILocationEventPublisher _locationEventPublisher;
        public LocationUpserter(ILocationRepository locationRepository,ILocationEventPublisher locationEventPublisher)
        {
            _locationRepository = locationRepository;
            _locationEventPublisher = locationEventPublisher;
        }

        public async Task<int> AddAsync(Location location)
        {
            if(location is null)
            throw new ArgumentNullException(nameof(location));
            var id = await _locationRepository.InsertLocationAsync(location);
            // await _locationEventPublisher.PublishAsync(new LocationEvent
            //     {
            //         EventType = "LocationInserted",
            //         Data = location
            //     });
            return id;
        }

        public async Task DeleteAsync(int id)
        {
            if (id < 0)
                throw new ArgumentNullException(nameof(id));
           await _locationRepository.DeleteLocationAsyncById(id);
        }

        public async Task<int> UpdateAsync(Location location)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location));
           var id = await _locationRepository.UpdateLocationAsync(location);
        //    await _locationEventPublisher.PublishAsync(new LocationEvent
        //         {
        //             EventType = "LocationUpdated",
        //             Data = location
        //         });
            return id;
        }
    }
}
