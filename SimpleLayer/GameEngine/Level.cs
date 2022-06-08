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
    public readonly int LevelWidth = 3200;
    public readonly int LevelHeight = 3200;
    public readonly int LevelStartX = 0;
    public readonly int LevelStartY = 0;
    public readonly int LevelEndX = 3200;
    public readonly int LevelEndY = 3200;
    private readonly Texture _textureManager;
    private IntPtr _renderer;
    public SDL_Rect _sRect, _dRect;
    public int[,] _levelMatrix;

    public Level(IntPtr renderer, Texture textureManager)
    {
        _textureManager = textureManager;
        _levelMatrix = new int[LevelWidth / 32, LevelHeight / 32];
        _renderer = renderer;
        _sRect.h = 32;
        _sRect.w = 32;
        _dRect.w = 32;
        _dRect.h = 32;
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
}