using ASPServerSignalR.DataStorage;
using Microsoft.AspNetCore.SignalR;

namespace ASPServerSignalR.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageStorage _storage;

    public ChatHub(IMessageStorage storage)
    {
        _storage = storage;
    }

    public async Task SendMessage(string user, string message)
    {
        Console.WriteLine(message);
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public void Publish(string topic, string username, string message)
    {
        _storage.AddMessages(username, message);
        Clients.Group(topic).SendAsync("SubscribeLister", new[] {_storage.GetMessages()});
    }

    public async Task Subscribe(string topic)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, topic);
    }

    public async Task Unsubscribe(string topic)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, topic);
    }

    public async Task SwapTopic(string currentTopic, string newTopic)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentTopic);
        await Groups.AddToGroupAsync(Context.ConnectionId, newTopic);
        await Clients.Caller.SendAsync("SubscribeLister", new[] {_storage.GetMessages()});
    }
}