
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text;

public class LocationEventPublisher: ILocationEventPublisher
{
    private readonly ServiceBusClient _client;
    private readonly string _topicName = "location-events";

    public LocationEventPublisher(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task PublishAsync(LocationEvent locationEvent)
    {
        var sender = _client.CreateSender(_topicName);
        var messageBody = JsonSerializer.Serialize(locationEvent);
        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));

        message.ApplicationProperties.Add("eventType", locationEvent.EventType);
        await sender.SendMessageAsync(message);
    }
}