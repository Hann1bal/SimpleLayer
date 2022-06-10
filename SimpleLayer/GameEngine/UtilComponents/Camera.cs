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
    private int _screenWidhth = 1920;
    private int _screenHeight = 1080;

    public Camera()
    {
        CameraRect = new SDL.SDL_Rect {h = _screenHeight, w = _screenWidhth, x = 0, y = 0};
    }

    public void Move(Enum flag, ref Level level)
    {
        switch (flag)
        {
            case CameraDerection.RIGHT:
                if (CameraRect.x + _screenWidhth < Level.LevelEndX)
                {
                    CameraRect.x += (CameraRect.w / 2) / 20;
                }
                else
                {
                    CameraRect.x = Level.LevelEndX - _screenWidhth;
                }

                break;
            case CameraDerection.LEFT:
                if (CameraRect.x <= Level.LevelStartX)
                {
                    CameraRect.x = Level.LevelStartX;
                }
                else
                {
                    CameraRect.x -= (CameraRect.w / 2) / 20;
                }

                break;
            case CameraDerection.DONW:
                if (CameraRect.y + _screenHeight >= Level.LevelEndY)
                {
                    CameraRect.y = Level.LevelEndY - _screenHeight;
                }
                else
                {
                    CameraRect.y += (CameraRect.h / 2) / 20;
                }

                break;
            case CameraDerection.UP:
                if (CameraRect.y <= Level.LevelStartY)
                {
                    CameraRect.y = Level.LevelStartY;
                }
                else
                {
                    CameraRect.y -= (CameraRect.h / 2) / 20;
                }

                break;
        }
    }
}