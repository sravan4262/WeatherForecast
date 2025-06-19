using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Business.Interfaces;
using WeatherForecast.Domain.DataAccess.Classes;
using WeatherForecast.Domain.DataAccess.Interfaces;

namespace WeatherForecast.Domain.Business.Classes
{
    public class WeatherUpserter : IWeatherUpserter
    {
        private readonly IWeatherForecastRepository _weatherForecastRepository;
        public WeatherUpserter(IWeatherForecastRepository weatherForecastRepository)
        {
            _weatherForecastRepository = weatherForecastRepository;
        }

        public async Task DeleteAsync(int id)
        {
            if (id < 0)
                throw new ArgumentNullException(nameof(id));
            await _weatherForecastRepository.DeleteWeatherForecstAsyncById(id);
        }
    }
}
