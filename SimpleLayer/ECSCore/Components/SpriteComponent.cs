using SDL2;

namespace SimpleLayer.ECSCore.Components;

public class SpriteComponent : Component

{
    public IntPtr Texture { get; set; }
    public int HealthPoint { get; set; }
    public string Name { get; set; }
    public SDL.SDL_Rect SdlRect { get; set; }

    public override void Dispose()
    {
        Dispose();
        GC.SuppressFinalize(this);
    }
}