using SDL2;

namespace SimpleLayer.GameEngine.Templates;

public class GameTemplate
{
    public SDL.SDL_WindowFlags Flags = SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN;

    //TODO Переделать под Json и сделать автоматический конетейнер который будет хранить конкретные реализации базовых игровых типов объектов
    public int ScreenHeight = 1080;
    public int ScreenWidth = 1920;
}