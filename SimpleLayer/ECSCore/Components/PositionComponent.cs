using System.Numerics;

namespace SimpleLayer.ECSCore.Components;

public class Position : Component
{
    public Vector2 BasePosition { get; set; }

    public override void Dispose()
    {
        Dispose();
        GC.SuppressFinalize(this);
    }
}