using SDL2;

namespace SimpleLayer.GameEngine.UtilComponents;

public class Texture
{
    private IntPtr _openSans;
    private readonly Dictionary<string, string> _pathList = new();
    public Dictionary<string, IntPtr> Dictionary = new();

    private void GetAllTexturePath()
    {
        foreach (var path in Directory.GetFiles(".\\Data\\Texture",
                     "*.png", SearchOption.AllDirectories))
            _pathList.Add(Path.GetFileName(path).Split(".").First(), path);
    }

    private void InitTextButtonTexture(string buttonText, string textureName, IntPtr renderer)

    {
        SDL.SDL_Color color;
        foreach (var state in new List<string> {"", "_pressed", "_focused"})
        {
            color = state switch
            {
                "_pressed" => new SDL.SDL_Color {a = 0, r = 0, b = 0, g = 255},
                "_focused" => new SDL.SDL_Color {a = 0, r = 0, b = 255, g = 0},
                _ => new SDL.SDL_Color {a = 0, r = 255, b = 0, g = 0}
            };
            var message =
                SDL_ttf.TTF_RenderText_Solid(_openSans, buttonText, color);
            var texture = SDL.SDL_CreateTextureFromSurface(renderer, message);
            Dictionary.Add($"{textureName}{state}", texture);
            SDL.SDL_FreeSurface(message);
        }
    }

    public void LoadTexture(IntPtr renderer)
    {
        GetAllTexturePath();
        foreach (var (key, value) in _pathList)
            Dictionary.Add(key.Split(".").First(), SDL_image.IMG_LoadTexture(renderer, value));

        var tmpTextAndName = new Dictionary<string, string>
        {
            {"playTextButton", "Play"},
            {"settingsTextButton", "Settings"},
            {"exitTextButton", "Exit"},
            {"resumeTextButton", "Resume"}
        };
        _openSans = SDL_ttf.TTF_OpenFont(".\\Data\\Fonts\\OpenSans.ttf", 10);
        foreach (var (key, value) in tmpTextAndName) InitTextButtonTexture(value, key, renderer);

        tmpTextAndName.Clear();
    }

    public void ClearAllTexture()
    {
        foreach (var (key, value) in Dictionary)
        {
            Dictionary.Remove(key);
            SDL.SDL_DestroyTexture(value);
        }
    }
}