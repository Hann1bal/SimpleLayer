using SDL2;

namespace SimpleLayer.Objects;

public class  Buttons: HudBaseObject
{
    public bool IsPressed = false;
    public bool IsFocused = false;
    public bool IsGameObject = false;

    public enum ButtonTextures
    {
        Focused = 1,
        Pressed = 2,
        Unfocused = 0
    }
    public Buttons(string textureName, SDL.SDL_Rect sRect, SDL.SDL_Rect dRect, bool isGameObject) : base(textureName, sRect, dRect)
    {
        IsGameObject = isGameObject;
    }

    public void UpdateTextureName(ButtonTextures textures)
    {
        switch (textures)
        {
            case ButtonTextures.Focused:
                CurrentTextureName = TextureName + "_focused";
                IsFocused = true;
                IsPressed = false;
                break;
            case ButtonTextures.Pressed:
                CurrentTextureName = TextureName + "_pressed";
                IsFocused = true;
                IsPressed = true;
                break;
            case ButtonTextures.Unfocused:
                CurrentTextureName = TextureName;
                IsFocused = false;
                IsPressed = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(textures), textures, null);
        }
    }
}