using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class ServerCore
{
    private static TcpListener tcpListener; // сервер для прослушивания
    private readonly List<ClientObject> clients = new(); // все подключения

    protected internal void AddConnection(ClientObject clientObject)
    {
        clients.Add(clientObject);
    }

    protected internal void RemoveConnection(string id)
    {
        var client = clients.FirstOrDefault(c => c.Id == id);
        if (client != null)
            clients.Remove(client);
    }

    protected internal void Listen()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, 27015);
            tcpListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                var tcpClient = tcpListener.AcceptTcpClient();
                var clientObject = new ClientObject(tcpClient, this);
                var clientThread = new Thread(clientObject.Process);
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Disconnect();
        }
    }
    
    protected internal void BroadcastMessage(byte[] data, string id)
    {
        foreach (var t in clients.Where(t => t.Id != id))
            t.Stream.Write(data, 0, data.Length); 
    }


    protected internal void Disconnect()
    {
        tcpListener.Stop(); 

        foreach (var t in clients)
            t.Close();

        Environment.Exit(0); 
    }
}