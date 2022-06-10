using SDL2;
using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class HudBaseObject : IGameBaseObject
{
    public string TextureName;
    public string CurrentTextureName;
    public SDL.SDL_Rect SRect;
    public SDL.SDL_Rect DRect;

    public HudBaseObject(string textureName, SDL.SDL_Rect sRect, SDL.SDL_Rect dRect)
    {
        TextureName = textureName;
        CurrentTextureName = TextureName;
        SRect = sRect;
        DRect = dRect;
    }

    ~HudBaseObject()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(TextureName);
        GC.SuppressFinalize(SRect);
        GC.SuppressFinalize(DRect);
        GC.Collect(GC.MaxGeneration);
    }
}