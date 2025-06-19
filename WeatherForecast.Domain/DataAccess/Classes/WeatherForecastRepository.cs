using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.DataAccess.Interfaces;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.DataAccess.Classes
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly WeatherForecastDbContext _weatherForecastDbContext;
        public WeatherForecastRepository(WeatherForecastDbContext weatherForecastDbContext)
        {
            _weatherForecastDbContext = weatherForecastDbContext;
        }

        public async Task DeleteWeatherForecstAsyncById(int id)
        {
            var weatherForecast = _weatherForecastDbContext.WeatherForecast.FirstOrDefault(wf => wf.Id == id);
            if (weatherForecast != null)
            {
                _weatherForecastDbContext.WeatherForecast.Remove(weatherForecast);
                await _weatherForecastDbContext.SaveChangesAsync();
            }
        }

        public async Task<int> InsertWeatherForecastAsync(Entities.WeatherForecast weatherForecast)
        {
            await _weatherForecastDbContext.WeatherForecast.AddAsync(weatherForecast);
            await _weatherForecastDbContext.SaveChangesAsync();
            return weatherForecast.Id;
        }
    }
}
