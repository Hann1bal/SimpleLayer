namespace SimpleLayer.GameEngine.Containers;

public class ManagersContainer:IBaseConteiner<IBaseManger>
{
    public Dictionary<string, IBaseManger> Manager;

    public ManagersContainer()
    {
        Manager = new Dictionary<string, IBaseManger>();
    }
    public void Add(string name, IBaseManger manager)
    {
        Manager.Add(name, manager);
    }

    public void Remove(string name, IBaseManger manager)
    {
        Manager.Remove(name);
    }
}