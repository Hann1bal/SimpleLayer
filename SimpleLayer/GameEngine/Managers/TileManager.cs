using SDL2;
using SimpleLayer.GameEngine.UtilComponents;

namespace SimpleLayer.GameEngine.Managers;

public class TileManager
{
    private static TileManager? _tileManager = null;
    private Dictionary<int, Tile> _tileList;
    private Texture _textureManager;
    private Level _level;

    public static TileManager GetInstance(ref Dictionary<int, Tile> tileManager, ref Texture textureManager, ref Level level)
    {
        if (_tileManager != null)
        {
            return _tileManager;
        }

        _tileManager = new TileManager(ref tileManager, ref textureManager, ref level);
        return _tileManager;
    }

    private TileManager(ref Dictionary<int, Tile> tiles, ref Texture textureManager, ref Level level)
    {
        var cnt = 1;
        _tileList = tiles;
        _textureManager = textureManager;
        _level = level;
        for (var x = 0; x < 486; x++)
        {

                _tileList.Add(cnt, new Tile(new SDL.SDL_Rect{h = 32,w = 32,x = 0,y = 0}, new SDL.SDL_Rect(), _textureManager.Dictionary[$"{cnt}"], cnt ));
                cnt++;
        }
    }
}