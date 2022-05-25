using System.Numerics;
using SDL2;

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
    public const int _levelWidth = 3200;
    public const int _levelHeight = 3200;
    public readonly int LevelStartX = 0;
    public readonly int LevelStartY = 0;
    public readonly int LevelEndX = 3200;
    public readonly int LevelEndY = 3200;
    private Texture _textureManager;
    private IntPtr _renderer;
    public SDL.SDL_Rect _sRect, _dRect;
    private int[,] _levelMatrix;
    private Vector2 tiles = new Vector2();
    public Level(IntPtr renderer, Texture textureManager)
    {
        _textureManager = textureManager;
        _levelMatrix = new int[_levelWidth / 32, _levelHeight / 32];
        _renderer = renderer;
        GenerateMatrixValue();
    }

    private void GenerateMatrixValue()
    {
        for (var x = 0; x <= (_levelWidth / 32) - 1; x++)
        {
            for (var y = 0; y <= (_levelHeight / 32) - 1; y++)
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
        for (int x = 0; x < (_levelWidth / 32); x++)
        {
            for (int y = 0; y < (_levelHeight / 32); y++)
            {
                switch (_levelMatrix[x, y])
                {
                    case 2:
                        _sRect.x = 470;
                        _sRect.y = 297;
                        _dRect.x = x * 32 - camera._cameraRect.x;
                        _dRect.y = y * 32 - camera._cameraRect.y;
                        SDL.SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _sRect, ref _dRect);
                        break;
                    case 1:
                        _sRect.x = 404;
                        _sRect.y = 231;
                        _dRect.x = x * 32 - camera._cameraRect.x;
                        _dRect.y = y * 32 - camera._cameraRect.y;
                        SDL.SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _sRect, ref _dRect);
                        break;
                    case 0:

                        _sRect.x = 107;
                        _sRect.y = 33;
                        _dRect.x = x * 32 - camera._cameraRect.x;
                        _dRect.y = y * 32 - camera._cameraRect.y;
                        SDL.SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _sRect, ref _dRect);
                        break;
                }
            }
        }
    }
}