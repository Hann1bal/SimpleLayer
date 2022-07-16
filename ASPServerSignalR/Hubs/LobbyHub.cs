using System;
using System.Threading.Tasks;
using ASPServerSignalR.DataStorage;
using Microsoft.AspNetCore.SignalR;

namespace ASPServerSignalR;

public class LobbyHub : Hub
{
    private readonly IMatchStorage _storage;

    public LobbyHub(IMatchStorage storage)
    {
        _storage = storage; 
    }

    public async Task SendMessage(string user, string message)
    {
        Console.WriteLine(message);
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public void Publish(string topic, string username,string message)
    {
        _storage.CreateMatch();
        Clients.Group(topic).SendAsync("SubscribeLister",new[] {_storage.GetMatchList()});
    }
    
    public async Task Subscribe(string topic)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, topic);
        Clients.Caller.SendAsync("ReciveMatchList", new[] {_storage.GetMatchList()});

    }
    
    public async Task Unsubscribe(string topic)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, topic);
    }
}