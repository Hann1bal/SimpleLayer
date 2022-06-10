using System.Diagnostics;
using System.Numerics;
using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class Unit : GameBaseObject, IDisposable
{
    private IntPtr _renderer;
    private const int Speed = 1;

    public Unit(string textureName, int xPos, int yPos,
        int healtPpoint, int team, int damage) :
        base(textureName, xPos, yPos, healtPpoint, team, damage)
    {
    }

    public new void Dispose()
    {
        _renderer = IntPtr.Zero;
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}