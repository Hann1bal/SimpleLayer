using SDL2;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class EconomyGameManager
{
    public int Gold { get; set; }
    public uint LastTick;

    public EconomyGameManager()
    {
        Gold = 50;
    }

    public void RunJob()
    {
        GetCache();
    }

    private void GetCache()
    {
        if (SDL.SDL_GetTicks() - 15000 <= LastTick) return;
        Gold++;
        LastTick = SDL.SDL_GetTicks();
        
    }
}