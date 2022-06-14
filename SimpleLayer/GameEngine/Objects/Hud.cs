using SDL2;

namespace SimpleLayer.Objects;

public class Hud : HudBaseObject
{
    private static Hud _hud;

    protected Hud(string textureName, SDL.SDL_Rect sRect, SDL.SDL_Rect dRect) : base(textureName, sRect, dRect)
    {
    }


    public static Hud GetInstance(string textureName, SDL.SDL_Rect sRect, SDL.SDL_Rect dRect)
    {
        return _hud ??= new Hud(textureName, sRect, dRect);
    }
}