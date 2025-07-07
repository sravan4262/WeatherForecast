using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class LocationInsertedEventConsumerWorker : BackgroundService
{
    private readonly ILogger<LocationInsertedEventConsumerWorker> _logger;
    private readonly ServiceBusProcessor _processor;

    public Worker(ILogger<LocationInsertedEventConsumerWorker> logger, ServiceBusClient client)
    {
        _logger = logger;

        _processor = client.CreateProcessor("location-events", subscriptionName: "LocationInserted");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);

        _logger.LogInformation("Worker started listening to Service Bus.");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var locationEvent = JsonSerializer.Deserialize<LocationEvent>(body);

        _logger.LogInformation("Received event {EventType} at {Timestamp}", locationEvent.EventType, locationEvent.Timestamp);

        // TODO: Add your business logic here

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing message");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping worker...");
        await _processor.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}
