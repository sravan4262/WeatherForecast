public interface ILocationEventPublisher
{
    Task PublishAsync(LocationEvent locationEvent);
}