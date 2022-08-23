using SimpleLayer.GameEngine.Managers;

namespace SimpleLayer.GameEngine.Containers;

public class GameStateContainer:IBaseConteiner<Enum>
{
    private Dictionary<string, Enum> GameState = new Dictionary<string, Enum>();

    public void Add(string name, Enum state)
    {
        GameState.Add(name, state);
    }

    public void Remove(string name, Enum state)
    {
        GameState.Remove(name);
    }
}