using SDL2;
using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class Game : IDisposable
{
    private IntPtr _window;
    private IntPtr _renderer;
    private bool _running = true;
    private IntPtr _image;
    public SDL_Rect _vpRect;
    private uint _frameStart;
    private static readonly int _fps = 144;
    private int _frameDelay = 1000 / _fps;
    private Texture _textureManager = new Texture();
    private uint _frameTime;
    private GameBaseObject _base;
    private GameBaseObject _base2;
    private int xPos, yPos;
    private Level _level;
    private Camera _camera;
    private List<Building> playersBuildings = new();
    private SDL_Rect mes = new SDL_Rect();
    private SDL_Color _color = new SDL_Color() {a = 0, r = 255, b = 0, g = 0};
    private IntPtr monserat;
    private IntPtr message;

    private void Init()
    {
        if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
        {
            Console.WriteLine($"There was an issue initializing SDL. {SDL_GetError()}");
        }

        _window = SDL_CreateWindow(
            "SDL .NET 6 Tutorial",
            SDL_WINDOWPOS_CENTERED,
            SDL_WINDOWPOS_CENTERED,
            1920,
            1080,
            SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS | SDL_WindowFlags.SDL_WINDOW_OPENGL);

        if (_window == IntPtr.Zero)
        {
            Console.WriteLine($"There was an issue creating the window. {SDL_GetError()}");
        }

        _renderer = SDL_CreateRenderer(
            _window,
            -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

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
        monserat =
            SDL_ttf.TTF_OpenFont("C:\\Users\\wertu\\SimpleLayer\\SimpleLayer\\Data\\Fonts\\OpenSans.ttf", 10);
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
                    yPos = e.button.y + _camera._cameraRect.y;
                    xPos = e.button.x + _camera._cameraRect.x;
                    playersBuildings.Add(new Building(ref _renderer, ref _textureManager, "necropolis", (int) xPos,
                        (int) yPos, 15));
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
                    }

                    break;
                case SDL_EventType.SDL_MOUSEWHEEL:
                    if (e.wheel.y > 0)
                    {
                    }

                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    break;
            }
        }
    }

    private void Update()
    {
        foreach (var building in playersBuildings)
        {
            building.Update();
            building.Spawn();
            building.MoveAllUnits();
            building.UpdateAllUnits();
        }
    }

    private void DrawText()
    {
        var cnt = 0;
        foreach (var build in playersBuildings)
        {
            message = SDL_ttf.TTF_RenderText_Solid(monserat,
                $"{build._textureName}{cnt}  - List with {build._units.Count}", _color
            );

            var textureWreed = SDL_CreateTextureFromSurface(_renderer, message);
            mes.x = 0; //controls the rect's x coorinate 
            mes.y = 0 + cnt * 100; // controls the rect's y coordinte
            mes.w = 1000; // controls the width of the rect
            mes.h = 100; // controls the height of the rect
            SDL_Rect newRectangle = new()
            {
                h = mes.h, w = mes.w, x = mes.x, y = mes.y
            };
            SDL_RenderCopy(_renderer, textureWreed, IntPtr.Zero, ref newRectangle);
            cnt++;
            SDL_DestroyTexture(textureWreed);
        }
    }

    private void Render()
    {
        SDL_RenderClear(_renderer);

        _level.DrawMap(_camera);
        foreach (var building in playersBuildings)
        {
            building.Render(ref _camera);
            building.RenderAllUnits(ref _camera);
        }

        DrawText();

        SDL_RenderPresent(_renderer);
    }

    public void Dispose()
    {
        _textureManager.ClearAllTexture();
        SDL_DestroyRenderer(_renderer);
        SDL_DestroyWindow(_window);
        SDL_Quit();
        GC.Collect();
    }

    public void Run()
    {
        Init();
        while (_running)
        {
            _frameStart = SDL_GetTicks();
            PollEvents();
            Update();
            Render();

            _frameTime = SDL_GetTicks() - _frameStart;
            if (_frameDelay > _frameTime)
            {
                SDL_Delay((uint) (_frameDelay - _frameTime));
            }
        }

        Dispose();
    }
}