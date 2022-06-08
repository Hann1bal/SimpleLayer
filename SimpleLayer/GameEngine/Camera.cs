using SDL2;

namespace SimpleLayer.GameEngine;

enum CameraDerection
{
    LEFT = 1,
    RIGHT = 2,
    DONW = 3,
    UP = 4
}

public class Camera
{
    private int _xCameraPos, _yCameraPos = 0;
    public SDL.SDL_Rect _cameraRect;
    private IntPtr _renderer;
    private int _screenWidhth = 1920;
    private int _screenHeight = 1080;

    public Camera(IntPtr renderer)
    {
        _renderer = renderer;
        _cameraRect = new SDL.SDL_Rect {h = _screenHeight, w = _screenWidhth, x = 0, y = 0};
    }

    public void Move(Enum flag, Level level)
    {
        switch (flag)
        {
            case CameraDerection.RIGHT:
                if (_cameraRect.x+_screenWidhth < Level.LevelEndX)
                {
                    _cameraRect.x += (_cameraRect.w / 2) / 20;
                }
                else
                {
                    _cameraRect.x = Level.LevelEndX-_screenWidhth;
                }

                break;
            case CameraDerection.LEFT:
                if (_cameraRect.x <= Level.LevelStartX)
                {
                    _cameraRect.x = Level.LevelStartX;
                }
                else
                {
                    _cameraRect.x -= (_cameraRect.w / 2) / 20;
                }

                break;
            case CameraDerection.DONW:
                if (_cameraRect.y +_screenHeight >= Level.LevelEndY)
                {
                    _cameraRect.y = Level.LevelEndY-_screenHeight;
                }
                else
                {
                    _cameraRect.y += (_cameraRect.h / 2) / 20;
                }

                break;
            case CameraDerection.UP:
                if (_cameraRect.y <= Level.LevelStartY)
                {
                    _cameraRect.y = Level.LevelStartY;
                }
                else
                {
                    _cameraRect.y -= (_cameraRect.h / 2) / 20;
                }

                break;
        }
    }
}