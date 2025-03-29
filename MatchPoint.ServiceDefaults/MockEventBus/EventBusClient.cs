using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace MatchPoint.ServiceDefaults.MockEventBus
{
    public class EventBusClient : IAsyncDisposable, IEventBusClient
    {
        private readonly string _brokerUrl;
        private readonly HttpClient _httpClient;
        private readonly HubConnection _hubConnection;
        private readonly Dictionary<string, Func<EventMessage, Task>> _handlers = [];

        public EventBusClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClients.EventBusBroker);
            _brokerUrl = "https://localhost:7015"; //httpClient.BaseAddress?.ToString() ?? "http://localhost:5000";
            //_brokerUrl = config["EventBus:BrokerUrl"] ?? "http://localhost:5000";
            //_httpClient = new HttpClient()
            //{
            //    BaseAddress = new Uri($"{_brokerUrl}/")
            //};
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{_brokerUrl}/eventshub")
                .Build();

            // Register the callback for the event
            _hubConnection.On("ReceiveMessage", async (string topic, EventMessage message) =>
            {
                var topicHandlers = _handlers.Where(record => record.Key.StartsWith($"{topic}-")).Select(record => record.Value);
                foreach (var handler in topicHandlers)
                {
                    await handler(message);
                }
            });

            _hubConnection.StartAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Subscribe to the Event Bus for the given topic and subscription id
        /// </summary>
        /// <param name="topic"> The topic for which to subscribe. </param>
        /// <param name="subscriptionId"> An id assigned to this subscription. </param>
        /// <param name="handler"> A handler to be executed when a message for the given topic is received. </param>
        public async Task SubscribeAsync(string topic, string subscriptionId, Func<EventMessage, Task> handler)
        {
            var key = $"{topic}-{subscriptionId}";
            if (!_handlers.ContainsKey(key))
            {
                _handlers[key] = handler;
                var request = new SubscriptionRequest(_hubConnection.ConnectionId!);
                await _httpClient.PostAsJsonAsync($"{_brokerUrl}/subscribe/{topic}", request);
            }
        }

        /// <summary>
        /// Unsubscribe the given subscription from the event bus for the given topic.
        /// </summary>
        /// <param name="topic"> The topic for which to unsubscribe. </param>
        /// <param name="subscriptionId"> The id of the subscription to remove. </param>
        public async Task UnsubscribeAsync(string topic, string subscriptionId)
        {
            var key = $"{topic}-{subscriptionId}";
            if (_handlers.Remove(key) && !_handlers.Keys.Any(k => k.StartsWith($"{topic}-")))
            {
                await _httpClient.DeleteAsync($"{_brokerUrl}/subscribe/{topic}/{_hubConnection.ConnectionId}");
            }
        }

        /// <summary>
        /// Publish a new message for the given topic. 
        /// </summary>
        /// <param name="topic"> The topic for which to publish a new message. </param>
        /// <param name="eventType"> The <see cref="EventType"/> of the event. </param>
        /// <param name="payload"> A given payload to send with the message. </param>
        public async Task PublishAsync(string topic, EventType eventType, object payload)
        {
            var message = new EventMessage(eventType, payload);
            await _httpClient.PostAsJsonAsync($"publish/{topic}", message);
        }

        /// <summary>
        /// On dispose, remove all handlers and unsubscribe all subscriptions.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            // Group by unique topics and send one DELETE per topic
            var uniqueTopics = _handlers.Keys.Select(k => k.Split('-')[0]).Distinct();
            foreach (var topic in uniqueTopics)
            {
                await _httpClient.DeleteAsync($"{_brokerUrl}/subscribe/{topic}/{_hubConnection.ConnectionId}");
            }
            _handlers.Clear();
            await _hubConnection.StopAsync();
            _httpClient.Dispose();
        }
    }
}
