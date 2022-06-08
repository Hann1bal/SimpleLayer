using SDL2;
using SimpleLayer.ECSCore.Components;

namespace SimpleLayer.ECSCore.System;

public class CollisionSystem : System
{
    public SDL.SDL_bool Collision = SDL.SDL_bool.SDL_FALSE;

    protected override void Update(Entity entity, float deltaTime)
    {
        var intPtr = entity.GetComponent<SpriteComponent>().SdlRect;
        Collision = SDL2.SDL.SDL_HasIntersection(ref intPtr,
            ref intPtr);
    }
}