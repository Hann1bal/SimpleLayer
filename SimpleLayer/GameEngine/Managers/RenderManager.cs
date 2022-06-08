using SDL2;
using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class RenderManager
{
    private IntPtr _window;
    private IntPtr _renderer;
    private Texture _textureManager;
    private List<Building> _buildings;
    private static RenderManager _renderManager;
    private Camera _camera;
    private Level _level;

    private RenderManager(ref IntPtr renderer, ref IntPtr window, ref List<Building> buildings,
        ref Texture textureManager, ref Camera camera, ref Level level)
    {
        _renderer = renderer;
        _window = window;
        _buildings = buildings;
        _textureManager = textureManager;
        _camera = camera;
        _level = level;
    }

    public static RenderManager GetInstance(ref IntPtr renderer, ref IntPtr window, ref List<Building> buildings,
        ref Texture textureManager, ref Camera camera, ref Level level)
    {
        if (_renderManager != null) return _renderManager;
        return _renderManager =
            new RenderManager(ref renderer, ref window, ref buildings, ref textureManager, ref camera, ref level);
    }

    public void RunManager()
    {
        Render();
    }


    private void DrawMap(Camera camera)
    {

        for (var x = 0; x < (_level.LevelWidth / 32); x++)
        {
            for (var y = 0; y < (_level.LevelHeight / 32); y++)
            {
                switch (_level._levelMatrix[x, y])
                {
                    case 2:
                        _level._sRect.x = 470;
                        _level._sRect.y = 297;
                        _level._dRect.x = x * 32 - camera.CameraRect.x;
                        _level._dRect.y = y * 32 - camera.CameraRect.y;
                        SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _level._sRect, ref _level._dRect);
                        break;
                    case 1:
                        _level._sRect.x = 404;
                        _level._sRect.y = 231;
                        _level._dRect.x = x * 32 - camera.CameraRect.x;
                        _level._dRect.y = y * 32 - camera.CameraRect.y;
                        SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _level._sRect, ref _level._dRect);
                        break;
                    case 0:

                        _level._sRect.x = 107;
                        _level._sRect.y = 33;
                        _level._dRect.x = x * 32 - camera.CameraRect.x;
                        _level._dRect.y = y * 32 - camera.CameraRect.y;
                        SDL_RenderCopy(_renderer, _textureManager.Dictionary["tilemap"], ref _level._sRect,
                            ref _level._dRect);
                        break;
                }
            }
        }
    }
    private void Render()
    {

        SDL_RenderClear(_renderer);
        
        DrawMap(_camera);

        Console.WriteLine("done");
        foreach (var b in _buildings.ToArray())
        {
            b.Render(ref _camera, ref _textureManager);
            b.RenderAllUnits(ref _camera, ref _textureManager);
        }

        // DrawText();
        // RenderMesh();

        SDL_RenderPresent(_renderer);
    }
    // private void DrawText()
    // {
    //     var mes = new SDL.SDL_Rect();
    //     var cnt = 0;
    //     foreach (var build in _playersBuildings.Where(b => b.IsFactory == false))
    //     {
    //         var message = SDL_ttf.TTF_RenderText_Solid(_monserat,
    //             $"{build._healtPpoint} in team {build._team}", _color);
    //         var texture = SDL_CreateTextureFromSurface(_renderer, message);
    //         mes.x = 0; //controls the rect's x coorinate 
    //         mes.y = 0 + cnt * 100; // controls the rect's y coordinte
    //         mes.w = 1000; // controls the width of the rect
    //         mes.h = 100; // controls the height of the rect
    //
    //         SDL_RenderCopy(_renderer, texture, IntPtr.Zero, ref mes);
    //         SDL_FreeSurface(message);
    //         SDL_DestroyTexture(texture);
    //         cnt++;
    //     }
    // }
}