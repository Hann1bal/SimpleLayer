// using System.Diagnostics;
// using Microsoft.AspNetCore.SignalR.Client;
//
// namespace SimpleLayer.GameEngine.Network;
//
// public class NetworkSignalRConnection
// {
//     private readonly HubConnection connection;
//
//     public NetworkSignalRConnection()
//     {
//         connection = new HubConnectionBuilder().WithUrl("localhost:5001").WithAutomaticReconnect().Build();
//
//     }
//
//     public static async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
//     {
//         // Keep trying to until we can start or the token is canceled.
//         while (true)
//         {
//             try
//             {
//                 await connection.StartAsync(token);
//                 Debug.Assert(connection.State == HubConnectionState.Connected);
//                 return true;
//             }
//             catch when (token.IsCancellationRequested)
//             {
//                 return false;
//             }
//             catch
//             {
//                 // Failed to connect, trying again in 5000 ms.
//                 Debug.Assert(connection.State == HubConnectionState.Disconnected);
//                 await Task.Delay(5000);
//             }
//         }
//     }
//
//
// }