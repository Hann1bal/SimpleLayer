using SDL2;
using SimpleLayer.GameEngine.UI.UIAttributes;

namespace SimpleLayer.GameEngine.UI.UIElements;

public class Slider
{
    public SliderAttribute SliderAttribute;
    public SDL.SDL_Rect SliderRect;
    public SDL.SDL_Rect SliderMaxLenghtRect;


    public Slider(ref SDL.SDL_Rect sliderRect, ref SDL.SDL_Rect sliderMaxLenghtRect, SDL.SDL_Color color)
    {
        SliderRect = sliderRect;
        SliderMaxLenghtRect = sliderMaxLenghtRect;
        SliderAttribute = new SliderAttribute() {Color = color, CurrentPosition = 0};
    }
    
}
