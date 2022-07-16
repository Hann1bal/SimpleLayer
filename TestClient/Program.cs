
using System;
using System.Threading;
using SimpleLayer.GameEngine.Network;

namespace TestClient;

public static class Programm
{
    private static readonly NetworkSignalR _networkSignalR = new NetworkSignalR();
    private static void Main(string[] args)
    {
        _networkSignalR.ConnectWithRetryAsync(new CancellationToken());
        while (true)
        {
            var message = Console.ReadLine();
            _networkSignalR.SendMessage(message);
        }
    }
}