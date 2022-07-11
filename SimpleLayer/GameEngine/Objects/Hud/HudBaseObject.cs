using SDL2;

namespace SimpleLayer.Objects;

public class HudBaseObject : IGameBaseObject
{
    public SDL.SDL_Rect DRect;
    public HudBaseObjectAttribute hudBaseObjectAttribute;
    public SDL.SDL_Rect SRect;

    public HudBaseObject(string textureName, SDL.SDL_Rect sRect, SDL.SDL_Rect dRect)
    {
        hudBaseObjectAttribute = new HudBaseObjectAttribute
            {TextureName = textureName, CurrentTextureName = textureName};
        SRect = sRect;
        DRect = dRect;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(hudBaseObjectAttribute);
        GC.SuppressFinalize(SRect);
        GC.SuppressFinalize(DRect);
        GC.Collect(GC.MaxGeneration);
    }

    ~HudBaseObject()
    {
        Dispose();
    }
}