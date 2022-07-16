using System;
using System.Collections.Generic;

namespace ASPServerSignalR.DataStorage;

public class MatchStorage : IMatchStorage
{
    private readonly List<GameMatch> _gameMatches;
    public void CreateMatch()
    {
        throw new NotImplementedException();
    }

    public void RemoveMatch()
    {
        throw new NotImplementedException();
    }

    public void ConnectToMatch()
    {
        throw new NotImplementedException();
    }

    public void DisconnectFromMatch()
    {
        throw new NotImplementedException();
    }

    public List<GameMatch> GetMatchList()
    {
        throw new NotImplementedException();
    }
}