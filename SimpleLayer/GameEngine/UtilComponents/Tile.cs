using SDL2;

namespace SimpleLayer.GameEngine.UtilComponents;

public class Tile : IDisposable
{
    private bool _isMoveble = true;
    public int Id;
    private readonly SDL.SDL_Rect _sdlDRect;
    private readonly SDL.SDL_Rect _sdlSRect;
    public readonly IntPtr _texture;

    public Tile(SDL.SDL_Rect sdlDRect, SDL.SDL_Rect sdlSRect, IntPtr texture, int id)
    {
        _sdlDRect = sdlDRect;
        _sdlSRect = sdlSRect;
        _texture = texture;
        Id = id;
    }

    ~Tile()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(_texture);
        GC.SuppressFinalize(_sdlDRect);
        GC.SuppressFinalize(_sdlSRect);
        GC.Collect(GC.MaxGeneration);
    }
}