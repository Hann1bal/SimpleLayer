using System.Diagnostics;
using System.Numerics;
using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class Unit : GameBaseObject, IDisposable
{
    private IntPtr _renderer;
    private const int Speed = 1;

    public Unit(ref IntPtr renderer, string textureName, int xPos, int yPos,
        int healtPpoint, int team) :
        base(ref renderer, textureName, xPos, yPos, healtPpoint, team)
    {
        _renderer = renderer;
    }


    public new void Render(ref Camera camera, ref Texture textureManager)
    {
        base.Render(ref camera, ref textureManager);
    }

    public new void Dispose()
    {
        _renderer = IntPtr.Zero;
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}