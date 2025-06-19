using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Areas.Location.V1.Mappers;
using WeatherForecast.Areas.Location.V1.ViewModels;
using WeatherForecast.Domain.Business.Interfaces;

namespace WeatherForecast.Areas.Location.V1.Controllers
{
    [ApiController]
    [Route("api/location/")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationUpserter _locationUpserter;
        private readonly ILocationRetriever _locationRetriever;
        private readonly ILocationViewModelMapper _viewModelMapper;

        public LocationController(ILocationRetriever locationRetriever, ILocationUpserter locationUpserter, ILocationViewModelMapper viewModelMapper)
        {
            _locationRetriever = locationRetriever;
            _locationUpserter = locationUpserter;
            _viewModelMapper = viewModelMapper;
        }


        /// <summary>
        /// Gets all locations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("locations")]
        [ProducesResponseType(typeof(List<Domain.Entities.Location>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _locationRetriever.GetLocationsAsync();
            var locationViewModels = _viewModelMapper.Map(locations);
            return Ok(locationViewModels);
        }

        /// <summary>
        /// Gets all locations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mostRecent")]
        [ProducesResponseType(typeof(List<Domain.Entities.Location>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetMostRecentLocation()
        {
            var location = await _locationRetriever.GetMostRecentAsync();
            var locationViewModel = _viewModelMapper.Map(location);
            return Ok(locationViewModel);
        }


        /// <summary>
        /// Gets location by id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("id")]
        [ProducesResponseType(typeof(List<Domain.Entities.Location>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetLocations(int id)
        {
            var location = await _locationRetriever.GetLocationAsyncById(id);
            var locationViewModel = _viewModelMapper.Map(location);
            return Ok(locationViewModel);
        }

        /// <summary>
        /// Inserts a Location
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AddLocation([FromBody] LocationInsertRequestModel insertRequestModel)
        {
            var location = _viewModelMapper.Map(insertRequestModel);
            var key = await _locationUpserter.AddAsync(location);
            return Ok(key);
        }

        /// <summary>
        /// Updates a Location
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationUpdateRequestModel updateRequestModel)
        {
            var location = _viewModelMapper.Map(updateRequestModel);
            var key = await _locationUpserter.UpdateAsync(location);
            return Ok(key);
        }

        /// <summary>
        /// Deletes a Location
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("id")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            await _locationUpserter.DeleteAsync(id);
            return Ok();
        }
    }
}
