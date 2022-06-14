using SDL2;

namespace SimpleLayer.Objects;

public class HudBaseObject : IGameBaseObject
{
    public string CurrentTextureName;
    public SDL.SDL_Rect DRect;
    public SDL.SDL_Rect SRect;
    public string TextureName;

    public HudBaseObject(string textureName, SDL.SDL_Rect sRect, SDL.SDL_Rect dRect)
    {
        TextureName = textureName;
        CurrentTextureName = TextureName;
        SRect = sRect;
        DRect = dRect;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(TextureName);
        GC.SuppressFinalize(SRect);
        GC.SuppressFinalize(DRect);
        GC.Collect(GC.MaxGeneration);
    }

    ~HudBaseObject()
    {
        Dispose();
    }
}