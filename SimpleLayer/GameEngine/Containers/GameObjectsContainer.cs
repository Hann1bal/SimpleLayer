namespace SimpleLayer.GameEngine.Containers;

public class GameObjectsContainer : IBaseConteiner<ISystemBaseObjects>
{
    public Dictionary<string, ISystemBaseObjects> SystemBaseObjectsMap = new Dictionary<string, ISystemBaseObjects>();

    public void Add(string name, ISystemBaseObjects manager)
    {
        SystemBaseObjectsMap.Add(name, manager);
    }

    public void Remove(string name, ISystemBaseObjects manager)
    {
        SystemBaseObjectsMap.Remove(name);
    }
}