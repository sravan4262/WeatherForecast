using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.Business.Interfaces
{
    public interface ILocationUpserter
    {
        Task<int> UpdateAsync(Location location);
        Task<int> AddAsync(Location location);
        Task DeleteAsync(int id);
    }
}
