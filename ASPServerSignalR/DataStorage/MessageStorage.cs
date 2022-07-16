using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASPServerSignalR.DataStorage;

public class MessageStorage : IMessageStorage
{
    private List<Tuple<string,string>> _messages;

    public MessageStorage()
    {
        
    }

    public async Task<bool> AddMessages(string username, string message)
    {
        try
        {
            _messages.Add(new Tuple<string, string>( username,  message));
            return true;
        }
        catch (Exception e)
        {
            return false;

        } 
    }

    public async Task<List<string>> GetMessages()
    {
        try
        {
            var messagePair = new List<string>();
            foreach (var pair in messagePair.ToArray())
            {
                messagePair.Add($"{pair[0]}: {pair[1]}");
            }

            return messagePair;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}