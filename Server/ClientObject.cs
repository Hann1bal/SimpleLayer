using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Server;

public class ClientObject
{
    private readonly TcpClient client;
    private readonly ServerCore server; // объект сервера
    private string userName;

    public ClientObject(TcpClient tcpClient, ServerCore serverObject)
    {
        Id = Guid.NewGuid().ToString();
        client = tcpClient;
        server = serverObject;
        serverObject.AddConnection(this);
    }

    protected internal string Id { get; }
    protected internal NetworkStream Stream { get; private set; }

    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        MessageId = "type: WhereListIterator`1[Server.ClientObject]")]
    public void Process()
    {
        try
        {
            Stream = client.GetStream();
            userName = Id;
            var message = userName + " Вошёл в игру";
            Console.WriteLine(message);
            var data = new byte[2048];
            var cnt = 0;
            var flag = true;
            while (flag)
                try
                {
                    data = GetMessage();
                    Console.WriteLine(data);
                    server.BroadcastMessage(data, Id);
                }
                catch
                {
                    Thread.Sleep(2000);
                    if (cnt < 5)
                    {
                        server.BroadcastMessage(data, Id);
                        cnt++;
                        break;
                    }

                    flag = false;
                    Console.WriteLine("Connection timeout");
                    break;
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            server.RemoveConnection(Id);
            Close();
        }
    }

    private byte[] GetMessage()
    {
        var data = new byte[2048];
        do
        {
            Stream.Read(data, 0, data.Length);
        } while (Stream.DataAvailable);

        return data;
    }

    // закрытие подключения
    protected internal void Close()
    {
        if (Stream != null)
            Stream.Close();
        if (client != null)
            client.Close();
    }
}