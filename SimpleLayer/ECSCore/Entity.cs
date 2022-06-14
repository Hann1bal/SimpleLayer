using SimpleLayer.ECSCore.Components;

namespace SimpleLayer.ECSCore;

public class Entity
{
    private readonly Dictionary<Type, Component> _components;

    public Entity(int id)
    {
        Id = id;
        _components = new Dictionary<Type, Component>();
    }

    public int Id { get; }

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