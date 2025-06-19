using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Areas.WeatherForecast.V1.Mappers;
using WeatherForecast.Domain.Business.Interfaces;

namespace WeatherForecast.Areas.WeatherForecast.V1.Controllers
{
    [ApiController]
    [Route("api/weatherForecast/")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherRetriever _weatherRetriever;
        private readonly IWeatherUpserter _weatherUpserter;
        private readonly IWeatherViewModelMapper _weatherViewModelMapper;

        public WeatherForecastController(IWeatherRetriever weatherRetriever, IWeatherViewModelMapper weatherViewModelMapper, IWeatherUpserter weatherUpserter)
        {
            _weatherRetriever = weatherRetriever;
            _weatherViewModelMapper = weatherViewModelMapper;
            _weatherUpserter = weatherUpserter;
        }

        /// <summary>
        /// Get the weather forecast by location id
        /// </summary>
        /// <param name="locationId">Location's id</param>
        /// <returns></returns>
        [HttpGet("locationId")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetForecastByLocation(int locationId)
        {
            var openMeteoResponse = await _weatherRetriever.GetWeatherForecastByLocationIdAsync(locationId);
            var weatherForecast = _weatherViewModelMapper.Map(openMeteoResponse);
            return Ok(weatherForecast);
        }

        /// <summary>
        /// Get the weather forecast by latitude and longitude
        /// Example : Delaware - Latitude: 39.7391, Longitude: -75.5398
        /// </summary>
        /// <param name="latitude">Location's latitude</param>
        /// <param name="longitude">Location's longitude</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetForecastByLatitudeLongitude(double latitude, double longitude)
        {
            var openMeteoResponse = await _weatherRetriever.GetWeatherForecastAsync(latitude, longitude);
            var weatherForecast = _weatherViewModelMapper.Map(openMeteoResponse);
            return Ok(weatherForecast);
        }

        /// <summary>
        /// Saves the weather forecast by location id
        /// </summary>
        /// <param name="latitude">Location's latitude</param>
        /// <param name="longitude">Location's longitude</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Insert(double latitude, double longitude)
        {
            var weatherId = await _weatherRetriever.AddAsync(latitude, longitude);
            return Ok(weatherId);
        }


        /// <summary>
        /// Deletes a weather forecast
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("id")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> DeleteWeatherForecast(int id)
        {
            await _weatherUpserter.DeleteAsync(id);
            return Ok();
        }
    }
}
