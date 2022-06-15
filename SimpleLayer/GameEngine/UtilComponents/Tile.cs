using SDL2;

namespace SimpleLayer.GameEngine.UtilComponents;

public class Tile : IDisposable
{
    public readonly IntPtr _texture;
    private bool _isMoveble = true;
    public SDL.SDL_Rect _sdlDRect;
    public SDL.SDL_Rect _sdlSRect;
    public bool ContainBuilding = false;
    public int Id;
    public bool isPlacibleTile = false;

    public Tile(SDL.SDL_Rect sdlDRect, SDL.SDL_Rect sdlSRect, IntPtr texture, int id)
    {
        _sdlDRect = sdlDRect;
        _sdlSRect = sdlSRect;
        _texture = texture;
        Id = id;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(_sdlSRect);
        GC.SuppressFinalize(_sdlDRect);
        GC.SuppressFinalize(_texture);
        GC.Collect(GC.MaxGeneration);
    }

    ~Tile()
    {
        Dispose();
    }
}