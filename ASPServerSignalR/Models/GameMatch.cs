namespace ASPServerSignalR;

public class GameMatch
{
    public Guid id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public List<Player> Players { get; set; }
}