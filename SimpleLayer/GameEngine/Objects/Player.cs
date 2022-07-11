namespace SimpleLayer.Objects;

public class Player
{
    public PlayerAttribute PlayerAttribute;

    public Player()
    {
        PlayerAttribute = new PlayerAttribute {Gold = 0, Nickname = "RusichRu", Team = 1};
    }
}