using System.Numerics;

namespace SimpleLayer.ECSCore.Components;

public class MovementComponent : Component
{
    public int Speed { get; set; }
    public int Acceleration { get; set; }
    public Vector2 Point { get; set; }
    public int DistanceToNearestEnemy { get; set; }

    public override void Dispose()
    {
        Dispose();
        GC.SuppressFinalize(this);
    }
}