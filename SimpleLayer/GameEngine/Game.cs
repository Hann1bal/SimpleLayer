using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using SDL2;
using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class Game : IDisposable
{
    private IntPtr _window;
    private IntPtr _renderer;
    private bool _running = true;
    private uint _frameStart;
    private const int Fps = 144;
    private const int FrameDelay = 1000 / Fps;
    private Texture _textureManager = new Texture();
    private uint _frameTime;
    private Level _level;
    private Camera _camera;
    private List<Building> _playersBuildings = new();
    private readonly SDL_Color _color = new SDL_Color() {a = 0, r = 255, b = 0, g = 0};
    private IntPtr _monserat;
    private GameLogicManager _gameLogicManager;

    private void Init()
    {
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"There was an issue initializing SDL. {SDL_GetError()}");
        }

        _window = SDL_CreateWindow(
            "SDL .NET 6 Tutorial",
            SDL_WINDOWPOS_CENTERED,
            SDL_WINDOWPOS_CENTERED,
            1920,
            1080,
            SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS | SDL_WindowFlags.SDL_WINDOW_VULKAN);

        if (_window == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the window. {SDL_GetError()}");
        }


        _renderer = SDL_CreateRenderer(
            _window,
            -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (_renderer == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the renderer. {SDL_GetError()}");
        }

        _textureManager.LoadTexture(_renderer);
        _level = new Level(_renderer, _textureManager);
        _camera = new Camera(_renderer);
        SDL_SetWindowGrab(_window, SDL_bool.SDL_TRUE);

        if (SDL_ttf.TTF_Init() < 0)
        {
            Console.WriteLine($"There was an issue creating the renderer. {SDL_ttf.TTF_GetError()}");
        }

        _monserat =
            SDL_ttf.TTF_OpenFont($"./Data/Fonts/OpenSans.ttf", 10);
        _gameLogicManager = GameLogicManager.GetInstance(ref _playersBuildings, _renderer);
    }


    private void PollEvents()
    {
        while (SDL_PollEvent(out SDL_Event e) == 1)
        {
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    _running = false;
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    Building building;
                    switch (e.button.x + _camera._cameraRect.x)
                    {
                        case < 800:
                            building = new Building(ref _renderer, "necropolis",
                                e.button.x + _camera._cameraRect.x, e.button.y + _camera._cameraRect.y, 15, 1);
                            _playersBuildings.Add(building);
                            _gameLogicManager.AddToQuadrant(building);
                            break;
                        case > 2400:
                            building = new Building(ref _renderer, "necropolis",
                                e.button.x + _camera._cameraRect.x, e.button.y + _camera._cameraRect.y, 15, 2);
                            _playersBuildings.Add(building);
                            _gameLogicManager.AddToQuadrant(building);
                            break;
                    }

                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    switch (e.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_LEFT:
                            _camera.Move(CameraDerection.LEFT, _level);
                            break;
                        case SDL_Keycode.SDLK_RIGHT:
                            _camera.Move(CameraDerection.RIGHT, _level);
                            break;
                        case SDL_Keycode.SDLK_UP:
                            _camera.Move(CameraDerection.UP, _level);
                            break;
                        case SDL_Keycode.SDLK_DOWN:
                            _camera.Move(CameraDerection.DONW, _level);
                            break;
                        default:
                            break;
                    }

                    break;
                case SDL_EventType.SDL_MOUSEWHEEL:
                    if (e.wheel.y > 0)
                    {
                        _camera.Move(CameraDerection.LEFT, _level);
                    }


                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    switch (e.motion.x)
                    {
                        case <= 2:
                            _camera.Move(CameraDerection.LEFT, _level);
                            break;
                        case >= 1915:
                            _camera.Move(CameraDerection.RIGHT, _level);
                            break;
                    }

                    switch (e.motion.y)
                    {
                        case <= 2:
                            _camera.Move(CameraDerection.UP, _level);
                            break;
                        case >= 1072:
                            _camera.Move(CameraDerection.DONW, _level);
                            break;
                    }


                    break;
            }
        }
    }


    private void RenderMesh()
    {
        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                SDL_RenderDrawLine(_renderer, i * 320 - _camera._cameraRect.x, j * 320 - _camera._cameraRect.y,
                    i * 320 - 320 - _camera._cameraRect.x, j * 320 - _camera._cameraRect.y);
                SDL_RenderDrawLine(_renderer, i * 320 - _camera._cameraRect.x, j * 320 - _camera._cameraRect.y,
                    i * 320 - _camera._cameraRect.x, j * 320 + 320 - _camera._cameraRect.y);
                SDL_RenderDrawLine(_renderer, i * 320 + 320 - _camera._cameraRect.x, j * 320 + _camera._cameraRect.y,
                    i * 320 + 320 - _camera._cameraRect.x, j * 320 + 320 - _camera._cameraRect.y);
                SDL_RenderDrawLine(_renderer, i * 320, j * 320 + 320 - _camera._cameraRect.y,
                    i * 320 + 320 - _camera._cameraRect.x, j * 320 + 320 - _camera._cameraRect.y);
            }
        }
    }

    private void DrawText()
    {
        var mes = new SDL_Rect();
        var cnt = 0;
        foreach (var build in _playersBuildings)
        {
            var message = SDL_ttf.TTF_RenderText_Solid(_monserat,
                $"{build._textureName}{cnt} contains {build.Units.Count}", _color);
            var texture = SDL_CreateTextureFromSurface(_renderer, message);
            mes.x = 0; //controls the rect's x coorinate 
            mes.y = 0 + cnt * 100; // controls the rect's y coordinte
            mes.w = 1000; // controls the width of the rect
            mes.h = 100; // controls the height of the rect

            SDL_RenderCopy(_renderer, texture, IntPtr.Zero, ref mes);
            SDL_FreeSurface(message);
            SDL_DestroyTexture(texture);
            cnt++;
        }
    }

    private void Render()
    {
        SDL_RenderClear(_renderer);
        _level.DrawMap(_camera);

        foreach (var b in _playersBuildings.ToArray())
        {
            b.Render(ref _camera, ref _textureManager);
            b.RenderAllUnits(ref _camera, ref _textureManager);
        }
        
        //DrawText();
        RenderMesh();

        SDL_RenderPresent(_renderer);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _textureManager.ClearAllTexture();
        SDL_DestroyRenderer(_renderer);
        SDL_DestroyWindow(_window);
        SDL_Quit();
        GC.SuppressFinalize(this);
        GC.Collect();
    }

    public void Run()
    {
        Init();
        while (_running)
        {
            var updateThread = new Thread(_gameLogicManager.RunManager);
            _frameStart = SDL_GetTicks();
            PollEvents();
            Render();
            updateThread.Start();
            _frameTime = SDL_GetTicks() - _frameStart;
            if (FrameDelay > _frameTime)
            {
                SDL_Delay((uint) (FrameDelay - _frameTime));
            }
        }

        Dispose();
    }
}