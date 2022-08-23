using System.Numerics;
using SDL2;
using SimpleLayer.GameEngine.Containers;
using SimpleLayer.GameEngine.UtilComponents;

namespace SimpleLayer.GameEngine.Managers;

public class TileManager :IBaseManger
{
    private static TileManager? _tileManager;
    private readonly Level _level;
    private readonly Texture _textureManager;
    private Dictionary<int, Tile> _tileList;

    private TileManager(ref Dictionary<int, Tile> tiles, ref Texture textureManager, ref Level level)
    {
        var cnt = 1;
        _tileList = tiles;
        _textureManager = textureManager;
        _level = level;
        Tile tile;
        for (var dx = 0; dx < Level.LevelEndX / 32; dx++)
        for (var dy = 0; dy < Level.LevelEndY / 32; dy++)
        {
            tile = new Tile(new SDL.SDL_Rect {h = 32, w = 32, x = dx * 32, y = dy * 32},
                new SDL.SDL_Rect {h = 32, w = 32, x = 0, y = 0},
                _textureManager.Dictionary[$"{_level._levelMatrix[dx, dy]}"], cnt);
            if (_level._levelMatrix[dx, dy] == 424) tile.TileAttribute.isPlacibleTile = true;
            _level._tileLevel.Add(new Vector2(dx, dy), tile);
            cnt++;
        }
    }

    public static TileManager GetInstance(ref Dictionary<int, Tile> tileManager, ref Texture textureManager,
        ref Level level)
    {
        if (_tileManager != null) return _tileManager;

        _tileManager = new TileManager(ref tileManager, ref textureManager, ref level);
        return _tileManager;
    }
}