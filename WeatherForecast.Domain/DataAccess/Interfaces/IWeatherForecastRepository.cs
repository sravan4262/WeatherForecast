using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.DataAccess.Interfaces
{
    public interface IWeatherForecastRepository
    {
        Task<int> InsertWeatherForecastAsync(Entities.WeatherForecast weatherForecast);
        Task DeleteWeatherForecstAsyncById(int id);
    }
}
