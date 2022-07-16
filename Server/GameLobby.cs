using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Server;

public class GameLobby
{
    private readonly ConcurrentDictionary<Guid, List<Player>> _lobbyList;
    private readonly int _maxRetryCount;
    /*
     * ConcurrentDictionary<Guid, ConcurrentDictionary<Type, ConcurrentBag<Connection>>>
     * Type -> enum
     * Player = 1
     * Observer = 2
     */
    public GameLobby(ConcurrentDictionary<Guid, List<Player>> lobbyList)
    {
        _lobbyList = lobbyList;
        _maxRetryCount = 5;
    }

    public void ConnectTo(Guid id, Player player)
    {
        _lobbyList[id].Add(player);
    }

    public void FindRandomOpponent(Player player)
    {
        var matches = _lobbyList.Where(c => c.Value.Count == 1).ToList();
        if (_lobbyList.Any() && _lobbyList.Any(c => c.Value.Count == 1))
            _lobbyList.TryAdd(new Guid(), new List<Player> {player});
        else
            matches.First().Value.Add(player);
    }

    public void CreateMatch(Player player)
    {
        _lobbyList.TryAdd(new Guid(), new List<Player> {player});
    }

    public void ClearEmptyLobby()
    {
        foreach (var variable in _lobbyList.Where(c => c.Value.Count == 0).ToList()) _lobbyList.TryRemove(variable);
    }

    public List<KeyValuePair<Guid, List<Player>>> GetAllMatches()
    {
        var matchList = _lobbyList.Where(c => c.Value.Count > 0).ToList();
        return matchList;
    }
}