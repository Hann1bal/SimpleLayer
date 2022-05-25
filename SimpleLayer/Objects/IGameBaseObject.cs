using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public interface IGameBaseObject : IDisposable
{
    public void Render(ref Camera camera);
    public void Update();
    public void Move();
    public void CheckCollision();
    
    public void Dispose();
}