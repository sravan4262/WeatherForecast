using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
namespace WeatherForecast.Function
{
    public class LocationUpdatedEventConsumerFunction
    {
        private readonly ILogger _logger;

        public LocationUpdatedEventConsumerFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LocationUpdatedEventConsumerFunction>();
        }

        [FunctionName("LocationUpdatedEventConsumerFunction")]
        public async Task RunAsync(
            [ServiceBusTrigger(
                topicName: "location-events",
                subscriptionName: "LocationUpdated",
                Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message)
        {
            try
            {
                string body = message.Body.ToString();
                var locationEvent = message.Body.ToObjectFromJson<LocationEvent>();

                _logger.LogInformation($"📦 Received LocationUpdatedEvent for eventtype = {locationEvent.EventType}");

                // TODO: Add your logic here to process the updated event
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing LocationUpdatedEvent.");
                throw; // Let Azure retry if something goes wrong
            }
        }
    }
}