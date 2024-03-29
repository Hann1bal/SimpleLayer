namespace ASPServerSignalR.DataStorage;

public interface IMessageStorage
{
    Task<List<string>> GetMessages();
    Task<bool> AddMessages(string username, string message);
}