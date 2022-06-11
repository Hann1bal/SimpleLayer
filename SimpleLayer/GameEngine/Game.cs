using SDL2;
using SimpleLayer.GameEngine.Managers;
using SimpleLayer.GameEngine.UtilComponents;
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
    private GameLogicManager _gameLogicManager;
    private RenderManager _rendererMeneger;
    private HudManager _hudMeneger;
    private TileManager _tileManager;
    private List<Buttons> _buttons = new();
    private bool _isPaused = false;
    private Hud _hud;
    private Building? _currentBuilding = null;
    private bool _isShiftPressed = false;
    private Dictionary<int, Tile> _tileList = new Dictionary<int, Tile>();

    private void Init()
    {
        if (SDL_Init(flags: SDL_INIT_EVERYTHING) < 0)
        {
            Console.WriteLine($"There was an issue initializing SDL. {SDL_GetError()}");
        }

        _window = SDL_CreateWindow(
            "Simple Test Game",
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
        _level = new Level();
        _camera = new Camera();
        SDL_SetWindowGrab(_window, SDL_bool.SDL_TRUE);

        if (SDL_ttf.TTF_Init() < 0)
        {
            Console.WriteLine($"There was an issue creating the renderer. {SDL_ttf.TTF_GetError()}");
        }

        SDL_ttf.TTF_OpenFont($"./Data/Fonts/OpenSans.ttf", 10);
        _gameLogicManager = GameLogicManager.GetInstance(ref _playersBuildings);
        _tileManager = TileManager.GetInstance(ref _tileList, ref _textureManager, ref _level);
        _hud = Hud.GetInstance("Hud", new SDL_Rect {x = 0, y = 0, h = 900, w = 1440},
            new SDL_Rect {x = 0, y = 0, w = _camera.CameraRect.w, h = _camera.CameraRect.h});
        _rendererMeneger = RenderManager.GetInstance(ref _renderer, ref _playersBuildings,
            ref _textureManager, ref _camera, ref _level, ref _buttons, ref _hud, ref _tileList);
        _hudMeneger = HudManager.GetInstance(ref _buttons, ref _hud);
    }


    private void PollEvents()
    {
        while (SDL_PollEvent(out var e) == 1)
        {
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    _running = false;
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (_currentBuilding != null)
                    {
                        _gameLogicManager.PlaceBuilding(e.button.x + _camera.CameraRect.x,
                            e.button.y + _camera.CameraRect.y, ref _currentBuilding);
                        if (!_isShiftPressed)
                        {
                            _currentBuilding.Dispose();
                            _currentBuilding = null;
                        }
                    }

                    foreach (var button in _buttons.Where(b => b.IsFocused))
                    {
                        _hudMeneger.PressButton(button, ref _isPaused, ref _currentBuilding);
                    }

                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    foreach (var button in _buttons.Where(b => b.IsPressed))
                    {
                        _hudMeneger.ReleaseButton(button);
                    }

                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    switch (e.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_LEFT:
                            _camera.Move(CameraDerection.LEFT, ref _level);
                            break;
                        case SDL_Keycode.SDLK_RIGHT:
                            _camera.Move(CameraDerection.RIGHT, ref _level);
                            break;
                        case SDL_Keycode.SDLK_UP:
                            _camera.Move(CameraDerection.UP, ref _level);
                            break;
                        case SDL_Keycode.SDLK_DOWN:
                            _camera.Move(CameraDerection.DONW, ref _level);
                            break;
                        case SDL_Keycode.SDLK_LSHIFT:
                            _isShiftPressed = true;
                            break;
                        default:
                            break;
                    }

                    break;
                case SDL_EventType.SDL_KEYUP:
                    switch (e.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_LSHIFT:
                            _isShiftPressed = false;
                            break;
                    }

                    break;
                case SDL_EventType.SDL_MOUSEWHEEL:
                    if (e.wheel.y > 0)
                    {
                        _camera.Move(CameraDerection.LEFT, ref _level);
                    }

                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    switch (e.motion.x)
                    {
                        case <= 2:
                            _camera.Move(CameraDerection.LEFT, ref _level);
                            break;
                        case >= 1915:
                            _camera.Move(CameraDerection.RIGHT, ref _level);
                            break;
                    }

                    switch (e.motion.y)
                    {
                        case <= 2:
                            _camera.Move(CameraDerection.UP, ref _level);
                            break;
                        case >= 1072:
                            _camera.Move(CameraDerection.DONW, ref _level);
                            break;
                    }

                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Run()
    {
        Init();
        while (_running)
        {
            _frameStart = SDL_GetTicks();
            _gameLogicManager.deltaTick = _frameStart;
            PollEvents();
            _hudMeneger.RunManager();
            var updateThread = new Thread(_gameLogicManager.RunManager);

            if (!_isPaused)
            {
                updateThread.Start();
            }

            _rendererMeneger.RunManager(ref _currentBuilding);
            if (_gameLogicManager.GetState())
            {
                _running = false;
            }

            _frameTime = SDL_GetTicks() - _frameStart;
            if (FrameDelay > _frameTime)
            {
                SDL_Delay(FrameDelay - _frameTime);
            }
        }

        Dispose();
    }

    public void Dispose()
    {
        _textureManager.ClearAllTexture();
        SDL_DestroyRenderer(_renderer);
        SDL_DestroyWindow(_window);
        SDL_Quit();
        GC.SuppressFinalize(this);
        GC.Collect();
    }
}