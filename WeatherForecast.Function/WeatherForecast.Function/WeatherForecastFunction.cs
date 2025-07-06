// WeatherForecast.Function/WeatherForecastFunction.cs
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherForecast.Function.Models; 

namespace WeatherForecast.Function
{
        public class WeatherForecastFunction
    {
        private readonly ILogger<WeatherForecastFunction> _logger;

        public WeatherForecastFunction(ILogger<WeatherForecastFunction> logger)
        {
            _logger = logger;
        }

        [FunctionName("weatherforecastfunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,            
            [CosmosDB(
                databaseName: "WeatherDb", // Replace with your desired Cosmos DB database name
                containerName: "WeatherEntries", // Replace with your desired Cosmos DB container name
                Connection = "CosmosDBConnection", // This refers to an app setting named "CosmosDBConnection"
                CreateIfNotExists = true,
                PartitionKey = "/partitionKey")] // Define the partition key path for the container
            IAsyncCollector<WeatherEntry> documentsOut) // Use IAsyncCollector for output binding
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            WeatherEntry data;

            try
            {
                data = JsonConvert.DeserializeObject<WeatherEntry>(requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing request body.");
                return new BadRequestObjectResult("Please pass a valid JSON object in the request body.");
            }

            if (data == null || string.IsNullOrEmpty(data.City) || string.IsNullOrEmpty(data.Summary))
            {
                return new BadRequestObjectResult("Please provide 'city' and 'summary' in the request body.");
            }

            // Set partition key if not already set by deserialization
            if (string.IsNullOrEmpty(data.PartitionKey))
            {
                data.PartitionKey = data.City; // Using City as partition key for this example
            }

            // Add the document to the output collector
            await documentsOut.AddAsync(data);
            _logger.LogInformation($"Weather entry for {data.City} created with ID: {data.Id}");

            return new OkObjectResult($"Weather entry for {data.City} created successfully with ID: {data.Id}");
        }
    }
}