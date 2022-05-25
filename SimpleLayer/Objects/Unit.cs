using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class Unit : GameBaseObject, IDisposable
{
    private int _xPos, _yPos;
    private IntPtr _renderer;
    public int _Speed = 1;

    public Unit(ref IntPtr renderer,ref Texture textureManager, string textureName, int xPos, int yPos,
        int healtPpoint) :
        base(ref renderer, ref textureManager, textureName, xPos, yPos, healtPpoint)
    {
        _renderer = renderer;
        _xPos = xPos;
        _yPos = yPos;
    }

    public void Move()
    {
        base.xPosition += 1 * _Speed;
    }

    public void Render(ref Camera camera)
    {
        base.Render(ref camera);
    }

    public void Dispose()
    {
        _renderer = IntPtr.Zero;
        base.Dispose();
    }
}