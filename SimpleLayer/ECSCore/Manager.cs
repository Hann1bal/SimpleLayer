namespace SimpleLayer.ECSCore;

public class Manager
{
    private Dictionary<int, Entity> entities;
    private Dictionary<Type, System.System> systems;
    private List<int> toDelete;
    private int currentId = 0;
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
        foreach (System.System system in systems.Values)
        {
            system.UpdateAll(deltaTime);
        }
        Flush();

    }
 
    private void Flush()
    {
        foreach (int id in toDelete)
        {
            if (!EntityExists(id)) //safeguard against deleting twice
                continue;
 
            foreach (System.System system in systems.Values)
            {
                system.DeleteEntity(id);
            }
 
            entities.Remove(id);
        }
        toDelete.Clear();
    }
    private void UpdateEntityRegistration(Entity entity)
    {
        foreach (System.System system in systems.Values)
        {
            system.UpdateEntityRegistration(entity);
        }
    }
}