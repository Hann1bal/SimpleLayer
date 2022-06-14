namespace SimpleLayer.ECSCore;

public class Manager
{
    private int currentId;
    private readonly Dictionary<int, Entity> entities;
    private readonly Dictionary<Type, System.System> systems;
    private readonly List<int> toDelete;

    public Manager()
    {
        entities = new Dictionary<int, Entity>();
        systems = new Dictionary<Type, System.System>();
        toDelete = new List<int>();
    }

    public void AddSystem(System.System system)
    {
        systems[system.GetType()] = system;
        system.BindManager(this);
    }

    public T GetSystem<T>() where T : System.System
    {
        return (T) systems[typeof(T)];
    }

    public bool EntityExists(int id)
    {
        return entities.ContainsKey(id);
    }

    public Entity AddAndGetEntity()
    {
        var entity = new Entity(currentId++);
        entities[entity.Id] = entity;
        return entity;
    }

    public void DeleteEntity(int id)
    {
        toDelete.Add(id);
    }

    public Entity GetEntityById(int id)
    {
        return entities[id];
    }

    public void Update(float deltaTime)
    {
        foreach (var system in systems.Values) system.UpdateAll(deltaTime);
        Flush();
    }

    private void Flush()
    {
        foreach (var id in toDelete)
        {
            if (!EntityExists(id)) //safeguard against deleting twice
                continue;

            foreach (var system in systems.Values) system.DeleteEntity(id);

            entities.Remove(id);
        }

        toDelete.Clear();
    }

    private void UpdateEntityRegistration(Entity entity)
    {
        foreach (var system in systems.Values) system.UpdateEntityRegistration(entity);
    }
}