using MatchPoint.ServiceDefaults.MockEventBus;

namespace MatchPoint.Api.Tests.Shared.Common.Helpers
{
    /// <summary>
    /// This class is used for automated testing to bypass actual functionality of the original class.
    /// This can be used when the original functionality can be ignored.
    /// </summary>
    public class TestEventBusClient : IEventBusClient
    {
        public Task PublishAsync(string topic, EventType eventType, object payload)
        {
            return Task.CompletedTask;
        }

        public Task SubscribeAsync(string topic, string subscriptionId, Func<EventMessage, Task> handler)
        {
            return Task.CompletedTask;
        }

        public Task UnsubscribeAsync(string topic, string subscriptionId)
        {
            return Task.CompletedTask;
        }
    }
}
