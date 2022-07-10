using SDL2;
namespace SimpleLayer.Objects;

public class Time
{
    public int Seconds = 0;
    private uint _deltaTime = 0;

    public void StartGameTimer()
    {
                
        if (SDL.SDL_GetTicks() - _deltaTime >= 1000)
        {
            Seconds += 1;
            _deltaTime = SDL.SDL_GetTicks();
        }
    }
}