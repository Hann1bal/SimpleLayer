using SDL2;

namespace SimpleLayer.GameEngine.UtilComponents;

public class Texture
{
    public Dictionary<string, IntPtr> Dictionary = new();
    private Dictionary<string, string> _pathList = new();

    private void GetAllTexturePath()
    {
        foreach (var path in Directory.GetFiles(".\\Data\\Texture",
                     "*.png", SearchOption.AllDirectories))
        {
            _pathList.Add(Path.GetFileName(path).Split(".").First(), path);
        }
    }

    public void CreateTextButton()
    {
        
    }
    public void LoadTexture(IntPtr renderer)
    {
        GetAllTexturePath();
        foreach (var (key, value) in _pathList)
        {
            Dictionary.Add(key.Split(".").First(), SDL_image.IMG_LoadTexture(renderer, value));
        }
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