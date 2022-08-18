namespace ASPServerSignalR.DataStorage;

public interface IMatchStorage
{
    void CreateMatch();
    void RemoveMatch();
    void ConnectToMatch();
    void DisconnectFromMatch();
    List<GameMatch> GetMatchList();
}