using SimpleLayer.ECSCore.Components;

namespace SimpleLayer.ECSCore.System;

public abstract class System
{
    private HashSet<int> registeredEntityIds;
    private List<Type> requiredComponents;
    protected Manager Manager;

    protected System()
    {
        registeredEntityIds = new HashSet<int>();
        requiredComponents = new List<Type>();
    }

    public void BindManager(Manager manager)
    {
        Manager = manager;
    }

    public void UpdateEntityRegistration(Entity entity)
    {
        bool matches = Matches(entity);
        if (registeredEntityIds.Contains(entity.Id))
        {
            if (!matches)
            {
                registeredEntityIds.Remove(entity.Id);
            }
        }
        else
        {
            if (matches)
            {
                registeredEntityIds.Add(entity.Id);
            }
        }
    }

    protected List<Entity> Entities
    {
        get
        {
            var result = from id in registeredEntityIds
                where Manager.EntityExists(id)
                select Manager.GetEntityById(id);

            return result.ToList();
        }
    }

    private bool Matches(Entity entity)
    {
        foreach (Type required in requiredComponents)
        {
            if (!entity.HasComponent(required))
                return false;
        }

        return true;
    }

    protected void AddRequiredComponent<T>() where T : Component
    {
        requiredComponents.Add(typeof(T));
    }

    public virtual void UpdateAll(float deltaTime)
    {
        foreach (Entity entity in Entities)
        {
            Update(entity, deltaTime);
        }
    }

    protected abstract void Update(Entity entity, float deltaTime);

    public virtual void DeleteEntity(int id)
    {
        if (registeredEntityIds.Contains(id))
        {
            registeredEntityIds.Remove(id);
        }
    }
}