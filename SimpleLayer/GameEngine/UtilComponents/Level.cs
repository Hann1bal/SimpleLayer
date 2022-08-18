using System.Numerics;
using SimpleLayer.GameEngine.Templates.LevelTemplates;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.UtilComponents;
//Grass
//x=107
//y=33

//Road
//x=470
//y=297

//Water
//x = 404
//y = 231

public class Level
{
    public const int LevelWidth = 3200;
    public const int LevelHeight = 3200;
    public const int LevelStartX = 0;
    public const int LevelStartY = 0;
    public const int LevelEndX = 3200;
    public const int LevelEndY = 3200;
    public int[,] _levelMatrix;

    public Dictionary<Vector2, Tile> _tileLevel = new();
    public LevelMapTemplate LevelMapTemplate = new();
    public SDL_Rect SRect, DRect;

    public Level()
    {
        _levelMatrix = new int[LevelWidth / 32, LevelHeight / 32];
        SRect.h = 32;
        SRect.w = 32;
        DRect.w = 32;
        DRect.h = 32;
        ConvertArrayToMatrix();
    }

    private void ConvertArrayToMatrix()
    {
        var cnt = 0;
        for (var x = 0; x <= LevelWidth / 32 - 1; x++)
        for (var y = 0; y <= LevelHeight / 32 - 1; y++)
        {
            _levelMatrix[y, x] = LevelMapTemplate._preGeneratedMap[cnt];
            cnt++;
        }
    }

    public Level GetCopy()
    {
        return (Level) MemberwiseClone();
    }
}