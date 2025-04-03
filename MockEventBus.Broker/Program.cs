using MatchPoint.ServiceDefaults.MockEventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MockEventBus.Broker;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddSignalR();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// Subscribe: Store client callback URL
app.MapPost("/subscribe/{topic}", async (string topic, [FromBody] SubscriptionRequest request, IHubContext<EventsHub, IEventsClient> hubContext) =>
{
    await hubContext.Groups.AddToGroupAsync(request.ClientId, topic);
    Console.WriteLine($"Subscribed {request.ClientId} to {topic}");
    return Results.Ok();
});

// Unsubscribe: Remove a subscription
app.MapDelete("/subscribe/{topic}/{clientId}", async (string topic, string clientId, IHubContext<EventsHub, IEventsClient> hubContext) =>
{
    await hubContext.Groups.RemoveFromGroupAsync(clientId, topic);
    Console.WriteLine($"Removed {clientId} from {topic}");
    return Results.Ok();
});

// Publish: Forward to subscribers
app.MapPost("/publish/{topic}", async (string topic, [FromBody] EventMessage message, IHubContext<EventsHub, IEventsClient> hubContext) =>
{
    await hubContext.Clients.Group(topic).ReceiveMessage(topic, message);
    return Results.Ok();
});

app.MapHub<EventsHub>("/eventshub");
app.Run();
