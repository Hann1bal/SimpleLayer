using System.Net.Sockets;
using System.Text;

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

    public void Process()
    {
        try
        {
            Stream = client.GetStream();
            // получаем имя пользователя
            userName = Id;
            var message = userName + " Вошёл в игру";
            Console.WriteLine(message);
            var data = new byte[64];
            while (true)
                try
                {
                    data = GetMessage();
                    Console.WriteLine(data);
                    server.BroadcastMessage(data, Id);
                }
                catch
                {
                    Console.WriteLine(data);
                    server.BroadcastMessage(data, Id);
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
        var data = new byte[64];
        do
        {
           var bytes = Stream.Read(data, 0, data.Length);
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