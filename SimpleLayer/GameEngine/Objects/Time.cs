using SDL2;

namespace SimpleLayer.GameEngine.Objects;

public class Time
{
    private uint _deltaTime;
    public int Seconds;

    public void StartGameTimer()
    {
        if (SDL.SDL_GetTicks() - _deltaTime < 1000) return;
        Seconds += 1;
        _deltaTime = SDL.SDL_GetTicks();
    }
}