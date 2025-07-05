// WeatherForecast.Function/WeatherForecastFunction.cs
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using WeatherForecast.Function.Models;

namespace WeatherForecast.Function
{
    public class WeatherForecastFunction
    {
        private readonly ILogger _logger;

        public WeatherForecastFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WeatherForecastFunction>();
        }
        
        [Function("weatherforecastfunction")]
        [CosmosDBOutput("WeatherDb", "WeatherEntries", Connection = "CosmosDBConnection", CreateIfNotExists = true, PartitionKey = "/partitionKey")]
        public async Task<WeatherEntry> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("weatherforecastfunction");
            logger.LogInformation("Processing request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            WeatherEntry data = null;
            try
            {
                data = JsonSerializer.Deserialize<WeatherEntry>(requestBody);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Invalid JSON");
                throw; // or handle with proper HTTP response as needed
            }

            if (string.IsNullOrEmpty(data.PartitionKey))
                data.PartitionKey = data.City;

            return data;  // returned object saved to Cosmos DB by binding
        }
    }
}
