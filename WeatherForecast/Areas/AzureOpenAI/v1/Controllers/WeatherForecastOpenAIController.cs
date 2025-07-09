using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Areas.WeatherForecast.V1.Mappers;
using WeatherForecast.Domain.Business.Interfaces;

namespace WeatherForecast.Areas.AzureOpenAI.v1.Controllers
{
    [ApiController]
    [Route("api/weatherForecastOpenAi/")]   
    public class WeatherForecatOpenAIController : ControllerBase
    {
        private readonly IAzureOpenAIService _openAiService;
        public WeatherForecatOpenAIController(IAzureOpenAIService openAIService)
        {
            _openAiService = openAIService;
        }

        [HttpPost]
        [Route("getLocationsByNaturalLanguage")]
        public async Task<IActionResult> AskLocations([FromBody] string userQuestion)
        {
            var response = await _openAiService.AskLocationsAsync(userQuestion);
            return Ok(response);
        }     
    }
}
