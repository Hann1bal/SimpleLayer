namespace SimpleLayer.GameEngine.Containers;

public interface IBaseConteiner<T> where T:class
{
     void Add(string name, T manager);
     void Remove(string name, T manager);
}