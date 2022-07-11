namespace Server;

public class GameLobby
{
    private readonly Dictionary<Guid, List<Player>> _lobbyList;

    public GameLobby(Dictionary<Guid, List<Player>> lobbyList)
    {
        _lobbyList = lobbyList;
    }

    public void ConnectTo(Guid id, Player player)
    {
        _lobbyList[id].Add(player);
    }

    public void FindOpponent(Player player)
    {
        var matches = _lobbyList.Where(c => c.Value.Count == 1).ToList();
        if (matches.Count == 0)
            _lobbyList.Add(new Guid(), new List<Player> {player});
        else
            matches.First().Value.Add(player);
    }

    public void CreateMatch(Player player)
    {
        _lobbyList.Add(new Guid(), new List<Player> {player});
    }

    public void ClearEmptyLobby()
    {
        foreach (var VARIABLE in _lobbyList.Where(c => c.Value.Count == 0).ToList()) _lobbyList.Remove(VARIABLE.Key);
    }

    public List<KeyValuePair<Guid, List<Player>>> GetAllMatches()
    {
        var matchList = _lobbyList.Where(c => c.Value.Count > 0).ToList();
        return matchList;
    }
}