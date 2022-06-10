using System.Numerics;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;
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
    public SDL_Rect SRect, DRect;
    public int[,] _levelMatrix;

    public Level()
    {
        _levelMatrix = new int[LevelWidth / 32, LevelHeight / 32];
        SRect.h = 32;
        SRect.w = 32;
        DRect.w = 32;
        DRect.h = 32;
        GenerateMatrixValue();
    }

    private void GenerateMatrixValue()
    {
        for (var x = 0; x <= (LevelWidth / 32) - 1; x++)
        {
            for (var y = 0; y <= (LevelHeight / 32) - 1; y++)
            {
                if (x is >= 25 and <= 68)
                {
                    _levelMatrix[x, y] = 0;
                }

                if (y <= 15 && x >= 25 || y >= 78 && x <= 69)
                {
                    _levelMatrix[x, y] = 1;
                }

                if (y >= 15 && x <= 25 || y <= 78 && x > 68)
                {
                    _levelMatrix[x, y] = 2;
                }
            }
        }
    }

    public Level GetCopy()
    {
        return (Level) this.MemberwiseClone();
    }
}