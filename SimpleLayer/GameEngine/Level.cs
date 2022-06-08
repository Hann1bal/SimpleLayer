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
    public const int LevelEndX = LevelStartX + LevelWidth;
    public const int LevelEndY = LevelStartY + LevelHeight;
    private readonly Texture _textureManager;
    private IntPtr _renderer;
    public SDL_Rect _sRect, _dRect;
    private int[,] _levelMatrix;

    public Level(IntPtr renderer, Texture textureManager)
    {
        _textureManager = textureManager;
        _levelMatrix = new int[LevelWidth / 32, LevelHeight / 32];
        _renderer = renderer;
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


    public void DrawMap(Camera camera)
    {
        _sRect.h = 32;
        _sRect.w = 32;
        _dRect.w = 32;
        _dRect.h = 32;
        for (var x = 0; x < (LevelWidth / 32); x++)
        {
            for (var y = 0; y < (LevelHeight / 32); y++)
            {
                switch (_levelMatrix[x, y])
                {
                    case 2:
                        _sRect.x = 470;
                        _sRect.y = 297;
                        _dRect.x = x * 32 - camera._cameraRect.x;
                        _dRect.y = y * 32 - camera._cameraRect.y;
                        SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _sRect, ref _dRect);
                        break;
                    case 1:
                        _sRect.x = 404;
                        _sRect.y = 231;
                        _dRect.x = x * 32 - camera._cameraRect.x;
                        _dRect.y = y * 32 - camera._cameraRect.y;
                        SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _sRect, ref _dRect);
                        break;
                    case 0:

                        _sRect.x = 107;
                        _sRect.y = 33;
                        _dRect.x = x * 32 - camera._cameraRect.x;
                        _dRect.y = y * 32 - camera._cameraRect.y;
                        SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _sRect, ref _dRect);
                        break;
                }
            }
        }
    }
}