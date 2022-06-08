using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public interface IGameBaseObject : IDisposable
{
    public void Render(ref Camera camera, ref Texture textureManager);
    public void Update();
    public void Move();
    public void Dispose();
}