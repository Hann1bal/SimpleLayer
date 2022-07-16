using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SimpleLayer.GameEngine.Network;

public class NetworkSignalR
{
    private readonly HubConnection connection;
    public HubConnectionState State;

    public NetworkSignalR()
    {
        connection = new HubConnectionBuilder().WithUrl("https://localhost:7071/ChatHub").WithAutomaticReconnect()
            .Build();
    }

    public async Task ConnectWithRetryAsync(CancellationToken token)
    {
        // Keep trying to until we can start or the token is canceled.
        while (true)
        {
            await connection.StartAsync(token);
            State = HubConnectionState.Connected;
            await connection.InvokeAsync("SendMessage", "RusichRu", "ping");
            Console.WriteLine("Connected");
        }
    }

    public async Task SendMessage(string message)
    {
        if (connection.State != HubConnectionState.Disconnected)
            await connection.InvokeAsync("SendMessage", "RusichRu", message);
        else
        {
            Console.WriteLine(connection.State);
        }
    }
}