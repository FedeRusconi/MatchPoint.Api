using Microsoft.AspNetCore.SignalR;

namespace MockEventBus.Broker
{
    public class EventsHub : Hub<IEventsClient>
    {
    }
}
