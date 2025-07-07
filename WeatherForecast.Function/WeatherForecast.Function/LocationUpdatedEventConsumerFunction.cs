public class LocationUpdatedEventConsumerFunction
    {
        private readonly ILogger _logger;

        public LocationUpdatedFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LocationUpdatedEventConsumerFunction>();
        }

        [Function("LocationUpdatedEventConsumerFunction")]
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
                var locationEvent = JsonSerializer.Deserialize<LocationEvent>(body);

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