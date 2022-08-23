using SDL2;
using SimpleLayer.GameEngine.Containers;
using SimpleLayer.GameEngine.Objects.Attributes;

namespace SimpleLayer.GameEngine.UtilComponents;

public class Tile : ISystemBaseObjects,IDisposable
{
    public SDL.SDL_Rect SdlDRect;
    public SDL.SDL_Rect SdlSRect;
    public TileAttribute TileAttribute;

    public Tile(SDL.SDL_Rect sdlDRect, SDL.SDL_Rect sdlSRect, IntPtr texture, int id)
    {
        SdlDRect = sdlDRect;
        SdlSRect = sdlSRect;
        TileAttribute = new TileAttribute {_texture = texture, Id = id};
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(SdlSRect);
        GC.SuppressFinalize(SdlDRect);
        GC.SuppressFinalize(TileAttribute._texture);
        GC.SuppressFinalize(TileAttribute);
        GC.Collect(GC.MaxGeneration);
    }

    ~Tile()
    {
        Dispose();
    }
}