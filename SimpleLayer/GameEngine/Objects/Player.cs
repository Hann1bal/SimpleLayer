using SimpleLayer.GameEngine.Objects.Attributes;

namespace SimpleLayer.GameEngine.Objects;

public class Player
{
    public PlayerAttribute PlayerAttribute;

    public Player()
    {
        PlayerAttribute = new PlayerAttribute {Gold = 50, Nickname = "RusichRu", Team = 1};
    }
}