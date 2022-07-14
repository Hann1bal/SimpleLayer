using SDL2;

namespace SimpleLayer.GameEngine.UI.UIAttributes;

public class SliderAttribute
{
    public  SDL.SDL_Color Color { get; init; }
    public int CurrentPosition { get; set; }

    public readonly int MaxPosition = 100;
    public readonly int MinPosition = 0;
}