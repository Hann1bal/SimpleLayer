using SimpleLayer.ECSCore.Components;

namespace SimpleLayer.ECSCore;

public class Entity
{
    public int Id { get; private set; }
    private Dictionary<Type, Component> _components;

    public Entity(int id)
    {
        Id = id;
        _components = new Dictionary<Type, Component>();
    }

    internal void AddComponent(Component component)
    {
        _components[component.GetType()] = component;
    }

    internal void RemoveComponent<T>() where T : Component
    {
        _components.Remove(typeof(T));
    }

    public T GetComponent<T>() where T : Component
    {
        return (T) _components[typeof(T)];
    }

    public bool HasComponent(Type componentType)
    {
        return _components.ContainsKey(componentType);
    }

    public bool HasComponent<T>() where T : Component
    {
        return _components.ContainsKey(typeof(T));
    }
}