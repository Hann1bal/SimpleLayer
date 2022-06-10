using SDL2;

namespace SimpleLayer.GameEngine.UtilComponents;

public class Minimap
{
    public SDL.SDL_Rect Sminmap;
    public SDL.SDL_Rect Dminmap;

    public Minimap()
    {
        Sminmap = new SDL.SDL_Rect {x = 0, y = 0, h = 220, w = 220};
        Dminmap = new SDL.SDL_Rect {x = 0, y = 1065, h = 320, w = 320};
    }
}