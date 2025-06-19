using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Business.Interfaces
{
    public interface IWeatherUpserter
    {
        Task DeleteAsync(int id);

    }
}
