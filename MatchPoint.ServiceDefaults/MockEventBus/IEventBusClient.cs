
namespace MatchPoint.ServiceDefaults.MockEventBus
{
    public interface IEventBusClient
    {
        Task PublishAsync(string topic, EventType eventType, object payload);
        Task SubscribeAsync(string topic, string subscriptionId, Func<EventMessage, Task> handler);
        Task UnsubscribeAsync(string topic, string subscriptionId);
    }
}