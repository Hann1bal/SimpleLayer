using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SimpleLayer.GameEngine.Network.EventModels;

namespace SimpleLayer.GameEngine.Managers;

public class NetworkManager
{
    private const string _host = "localhost";
    private const int _port = 5001;
    private static NetworkManager _manager;
    private static string userName;
    private static TcpClient client;
    private static NetworkStream stream;
    private readonly Stack<BuildingEvent> _events;
    private readonly Stack<BuildingEvent> _recieveEvents;
    private Thread _thread;
    private bool IsActiveThread;
    private bool IsConnected;

    private NetworkManager(ref Stack<BuildingEvent> events, ref Stack<BuildingEvent> recieveEvents)
    {
        _events = events;
        _recieveEvents = recieveEvents;
    }

    public static NetworkManager GetInstance(ref Stack<BuildingEvent> events, ref Stack<BuildingEvent> recieveEvents)
    {
        return _manager ?? new NetworkManager(ref events, ref recieveEvents);
    }

    public void RunManger()
    {
        if (!IsConnected) Connect();
        SendEvent();
        if (IsActiveThread) return;
        _thread = new Thread(RecieveEvents);
        _thread.Start();
        IsActiveThread = true;
    }

    private void SendEvent()
    {
        if (_events.Count == 0) return;
        while (_events.Count > 0)
        {
            var userEvent = _events.Pop();
            var data = new BinaryFormatter();
            using var memoryStream = new MemoryStream();
            data.Serialize(memoryStream, userEvent);
            var message = memoryStream.ToArray();
            stream.Write(message, 0, message.Length);
        }
    }

    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        MessageId = "type: System.Byte[]")]
    private void RecieveEvents()
    {
        while (IsConnected)
            try
            {
                var message = new byte[2048]; // буфер для получаемых данных
                while (stream.DataAvailable)
                {
                    stream.Read(message, 0, message.Length);
                    using var stream2 = new MemoryStream(message, 0, message.Length);
                    var formatter = new BinaryFormatter();
                    var recieveEvent = formatter.Deserialize(stream2);
                    _recieveEvents.Push((BuildingEvent) recieveEvent);
                }

                // Console.WriteLine(message); //вывод сообщения
            }
            catch
            {
                Console.WriteLine("Подключение прервано!"); //соединение было прервано
                Console.ReadLine();
                Disconnect();
            }
    }

    private void Connect()
    {
        client = new TcpClient();
        client.Connect(_host, _port);
        stream = client.GetStream(); // получаем поток
        IsConnected = true;
    }

    public void Disconnect()
    {
        IsConnected = false;
        if (stream != null)
            stream.Close(); //отключение потока
        if (client != null)
            client.Close(); //отключение клиента
        Environment.Exit(0); //завершение процесса
    }
}