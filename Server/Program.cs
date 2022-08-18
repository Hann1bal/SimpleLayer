// See https://aka.ms/new-console-template for more information


using Server;

internal class Program
{
    private static ServerCore server; // сервер
    private static Thread listenThread; // потока для прослушивания

    private static void Main(string[] args)
    {
        try
        {
            server = new ServerCore();
            listenThread = new Thread(server.Listen);
            listenThread.Start(); //старт потока    
        }
        catch (Exception ex)
        {
            server.Disconnect();
            Console.WriteLine(ex.Message);
        }
    }
}