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
        // получаем по id закрытое подключение
        var client = clients.FirstOrDefault(c => c.Id == id);
        // и удаляем его из списка подключений
        if (client != null)
            clients.Remove(client);
    }

    // прослушивание входящих подключений
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

    // трансляция сообщения подключенным клиентам
    protected internal void BroadcastMessage(string message, string id)
    {
        var data = Encoding.Unicode.GetBytes(message);
        for (var i = 0; i < clients.Count; i++)
            if (clients[i].Id != id) // если id клиента не равно id отправляющего
                clients[i].Stream.Write(data, 0, data.Length); //передача данных
    }

    // отключение всех клиентов
    protected internal void Disconnect()
    {
        tcpListener.Stop(); //остановка сервера

        for (var i = 0; i < clients.Count; i++) clients[i].Close(); //отключение клиента

        Environment.Exit(0); //завершение процесса
    }
}