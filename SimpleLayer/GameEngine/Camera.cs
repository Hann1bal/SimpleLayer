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
    public SDL.SDL_Rect CameraRect;
    private IntPtr _renderer;
    private int _screenWidhth = 1920;
    private int _screenHeight = 1080;

    public Camera(IntPtr renderer)
    {
        _renderer = renderer;
        CameraRect = new SDL.SDL_Rect {h = _screenHeight, w = _screenWidhth, x = 0, y = 0};
    }

    public void Move(Enum flag, ref Level level)
    {
        switch (flag)
        {
            case CameraDerection.RIGHT:
                if (CameraRect.x+_screenWidhth < level.LevelEndX)
                {
                    CameraRect.x += (CameraRect.w / 2) / 20;
                }
                else
                {
                    CameraRect.x = level.LevelEndX-_screenWidhth;
                }

                break;
            case CameraDerection.LEFT:
                if (CameraRect.x <= level.LevelStartX)
                {
                    CameraRect.x = level.LevelStartX;
                }
                else
                {
                    CameraRect.x -= (CameraRect.w / 2) / 20;
                }

                break;
            case CameraDerection.DONW:
                if (CameraRect.y +_screenHeight >= level.LevelEndY)
                {
                    CameraRect.y = level.LevelEndY-_screenHeight;
                }
                else
                {
                    CameraRect.y += (CameraRect.h / 2) / 20;
                }

                break;
            case CameraDerection.UP:
                if (CameraRect.y <= level.LevelStartY)
                {
                    CameraRect.y = level.LevelStartY;
                }
                else
                {
                    CameraRect.y -= (CameraRect.h / 2) / 20;
                }

                break;
        }
    }
}