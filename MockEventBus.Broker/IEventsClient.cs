using MatchPoint.ServiceDefaults.MockEventBus;

namespace MockEventBus.Broker
{
    public interface IEventsClient
    {
        Task ReceiveMessage(string topic, EventMessage message);
    }
}
