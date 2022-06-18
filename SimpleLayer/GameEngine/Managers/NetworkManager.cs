using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine.Managers;

public class NetworkManager
{
    private static NetworkManager _manager;
    private const string _host = "127.0.0.1";
    private const int _port = 27015;
    private static string userName;
    private static TcpClient client;
    private static NetworkStream stream;
    private Stack<Event> _events;
    private Stack<Event> _recieveEvents;
    private bool IsActiveThread = false;
    private bool IsConnected = false;
    private Thread _thread;
    private NetworkManager(ref Stack<Event> events, ref Stack<Event> recieveEvents)
    {
        _events = events;
        _recieveEvents = recieveEvents;
    }

    public static NetworkManager GetInstance(ref Stack<Event> events, ref Stack<Event> recieveEvents)
    {
        return _manager ?? new NetworkManager(ref events, ref recieveEvents);
    }

    public void RunManger()
    {   if(!IsConnected) Connect();
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

    private void RecieveEvents()
    {
        while (IsConnected)
            try
            {
                var message = new byte[1024]; // буфер для получаемых данных
                do
                {
                    var formatter = new BinaryFormatter();
                    var recieveEvent = (Event) formatter.Deserialize(stream);
                    _recieveEvents.Push(recieveEvent);
                } while (stream.DataAvailable);

                Console.WriteLine(message); //вывод сообщения
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
        var userName = Guid.NewGuid().ToString();
        stream.Write(Encoding.Unicode.GetBytes(userName), 0, userName.Length);
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