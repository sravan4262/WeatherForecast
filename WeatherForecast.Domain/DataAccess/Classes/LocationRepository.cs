using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.DataAccess.Interfaces;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.DataAccess.Classes
{
    public class LocationRepository : ILocationRepository
    {
        private readonly WeatherForecastDbContext _weatherForecastDbContext;
        public LocationRepository(WeatherForecastDbContext weatherForecastDbContext)
        {
            _weatherForecastDbContext = weatherForecastDbContext;
        }

        public async Task DeleteLocationAsyncById(int id)
        {
            var location = _weatherForecastDbContext.Location.FirstOrDefault(location => location.Id == id);
            if (location != null)
            {
                _weatherForecastDbContext.Location.Remove(location);
               await _weatherForecastDbContext.SaveChangesAsync();
            }
        }

        public async Task<Location?> GetLocationAsyncById(int id)
        {
            return await _weatherForecastDbContext.Location.FirstOrDefaultAsync(location => location.Id == id);
        }
        public async Task<int?> GetLocationIdByLatitudeLongiture(double latitude, double longitude)
        {
            var location = await _weatherForecastDbContext.Location
                .FirstOrDefaultAsync(location => location.Latitude == latitude && location.Longitude == longitude);

            return location?.Id;
        }

        public async Task<List<Location>> GetLocationsAsync()
        {
            return await _weatherForecastDbContext.Location.OrderByDescending(location => location.AccessedDateTime).ToListAsync();
        }

        public async Task<Location> GetMostRecentAsync()
        {
            return await _weatherForecastDbContext.Location.Include(l => l.WeatherForecasts).OrderByDescending(location => location.AccessedDateTime).FirstOrDefaultAsync();
        }

        public async Task<int> InsertLocationAsync(Location location)
        {
            await _weatherForecastDbContext.Location.AddAsync(location);
            await _weatherForecastDbContext.SaveChangesAsync();
            return location.Id;
        }

        public async Task<int> UpdateLocationAsync(Location location)
        {
            _weatherForecastDbContext.Location.Attach(location);
            _weatherForecastDbContext.Entry(location).State = EntityState.Modified;            
            await _weatherForecastDbContext.SaveChangesAsync();
            return location.Id;
        }
    }
}
