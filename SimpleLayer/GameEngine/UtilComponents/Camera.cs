using SDL2;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Templates;

namespace SimpleLayer.GameEngine.UtilComponents;

public class Camera
{
    private readonly GameTemplate _gameTemplate = new();
    public SDL.SDL_Rect CameraRect;

    public Camera()
    {
        CameraRect = new SDL.SDL_Rect {h = _gameTemplate.ScreenHeight, w = _gameTemplate.ScreenWidth, x = 0, y = 0};
    }

    public void Move(Enum flag)
    {
        switch (flag)
        {
            case CameraDirectionState.Right:
                if (CameraRect.x + _gameTemplate.ScreenWidth < Level.LevelEndX)
                    CameraRect.x += CameraRect.w / 2 / 20;
                else
                    CameraRect.x = Level.LevelEndX - _gameTemplate.ScreenWidth;

                break;
            case CameraDirectionState.Left:
                if (CameraRect.x <= Level.LevelStartX)
                    CameraRect.x = Level.LevelStartX;
                else
                    CameraRect.x -= CameraRect.w / 2 / 20;

                break;
            case CameraDirectionState.Donw:
                if (CameraRect.y + _gameTemplate.ScreenHeight >= Level.LevelEndY)
                    CameraRect.y = Level.LevelEndY - _gameTemplate.ScreenHeight;
                else
                    CameraRect.y += CameraRect.h / 2 / 20;

                break;
            case CameraDirectionState.Up:
                if (CameraRect.y <= Level.LevelStartY)
                    CameraRect.y = Level.LevelStartY;
                else
                    CameraRect.y -= CameraRect.h / 2 / 20;

                break;
        }
    }
}