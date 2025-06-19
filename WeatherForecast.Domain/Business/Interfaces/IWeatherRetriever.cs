using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.OpenMeteo;

namespace WeatherForecast.Domain.Business.Interfaces
{
    public interface IWeatherRetriever
    {
        Task<OpenMeteoResponse> GetWeatherForecastByLocationIdAsync(int locationId);
        Task<OpenMeteoResponse> GetWeatherForecastAsync(double latitude, double longitude, bool insertLocation = true);
        Task<int> AddAsync(double latitude, double longitude);

    }
}
