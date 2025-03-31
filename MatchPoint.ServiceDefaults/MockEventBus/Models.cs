namespace MatchPoint.ServiceDefaults.MockEventBus
{
    public record SubscriptionRequest(string ClientId);
    public record EventMessage(EventType EventType, object Payload);
}
