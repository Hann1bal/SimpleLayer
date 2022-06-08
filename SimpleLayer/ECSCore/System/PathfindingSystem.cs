using System.Numerics;
using SimpleLayer.ECSCore.Components;

namespace SimpleLayer.ECSCore.System;

public class PathfindingSystem : System
{
    protected override void Update(Entity entity, float deltaTime)
    {
        entity.GetComponent<MovementComponent>().DistanceToNearestEnemy =
            (int) Vector2.Distance(entity.GetComponent<Position>().BasePosition,
                entity.GetComponent<Position>().BasePosition);
    }
}