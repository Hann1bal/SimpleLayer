using SDL2;
using SimpleLayer.GameEngine.Managers;
using SimpleLayer.GameEngine.UtilComponents;
using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class Game : IDisposable
{
    public enum GameState
    {
        Menu = 0,
        Play = 1,
        GameOver = 2,
        Lobby = 3,
        Init = 4,
        Exit = 5
    }

    // Инициализация констант графического движка
    private const int ScreenHeight = 1080;
    private const int ScreenWidth = 1920;
    private const int Fps = 60;
    private const int FrameDelay = 1000 / Fps;

    // Инициализация списков игровых объектов
    private List<Buttons> _buttons = new();
    private List<Building> _playersBuildings = new();
    private List<Unit> _playersUnits = new();

    // Инициализация словарей игровых объектов
    private Dictionary<int, Tile> _tiles = new();

    // Инициализация системных объектов
    private uint _frameStart;
    private uint _frameTime;
    private bool _isPaused;
    private bool _isShiftPressed;
    private bool _matchState;
    private bool _running = true;
    private GameState _gameState;
    private IntPtr _renderer;
    private IntPtr _window;

    //Инициализация игровых менеджеров
    private RenderManager _rendererMeneger;
    private GameLogicManager _gameLogicManager;
    private HudManager _hudMeneger;
    private TileManager _tileManager;

    //Инициализация игровых объектов
    private Building? _currentBuilding;
    private Camera _camera;
    private Hud _hud;
    private Level _level;
    private Texture _textureManager =new ();


    public Game()
    {
        InitSdl();
        InitGameObjects();
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

    private void InitSdl()
    {
        if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
            Console.WriteLine($"There was an issue initializing SDL. {SDL_GetError()}");

        _window = SDL_CreateWindow(
            "Simple Test Game",
            SDL_WINDOWPOS_CENTERED,
            SDL_WINDOWPOS_CENTERED,
            ScreenWidth,
            ScreenHeight,
            SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS | SDL_WindowFlags.SDL_WINDOW_VULKAN);

        if (_window == IntPtr.Zero) Console.WriteLine($"There was an issue creating the window. {SDL_GetError()}");

        if (SDL_ttf.TTF_Init() < 0)
            Console.WriteLine($"There was an issue creating the renderer. {SDL_ttf.TTF_GetError()}");


        _renderer = SDL_CreateRenderer(
            _window,
            -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (_renderer == IntPtr.Zero) Console.WriteLine($"There was an issue creating the renderer. {SDL_GetError()}");

        SDL_SetWindowGrab(_window, SDL_bool.SDL_TRUE);

        if (SDL_ttf.TTF_Init() < 0) Console.WriteLine($"There was an issue creating the renderer. {SDL_ttf.TTF_GetError()}");
    }

    private void InitGameObjects()
    {
        _gameState = GameState.Init;
        _hud = Hud.GetInstance("Hud", new SDL_Rect {x = 0, y = 0, h = 900, w = 1440},
            new SDL_Rect {x = 0, y = 0, w = ScreenWidth, h = ScreenHeight});
        _textureManager.LoadTexture(_renderer);
        _hudMeneger = HudManager.GetInstance(ref _buttons, ref _hud, ref _gameState);
        _level = new Level();
        _camera = new Camera();
        _tileManager = TileManager.GetInstance(ref _tiles, ref _textureManager, ref _level);
        _rendererMeneger = RenderManager.GetInstance(ref _renderer, ref _playersBuildings,
            ref _textureManager, ref _camera, ref _level, ref _buttons, ref _hud, ref _tiles, ref _playersUnits);
        _gameLogicManager = GameLogicManager.GetInstance(ref _playersBuildings, ref _playersUnits);
        _gameState = GameState.Menu;
    }


    private void PollEvents()
    {
        while (SDL_PollEvent(out var e) == 1)
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    _running = false;
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (e.button.button != 3)
                    {
                        if (_currentBuilding != null)
                        {
                            _gameLogicManager.BuildingWorker.PlaceBuilding(e.button.x + _camera.CameraRect.x,
                                e.button.y + _camera.CameraRect.y, ref _currentBuilding, ref _level);
                            if (!_isShiftPressed)
                            {
                                _currentBuilding.Dispose();
                                _currentBuilding = null;
                            }
                        }
                    }
                    else
                    {
                        if (_currentBuilding != null)
                        {
                            _currentBuilding.Dispose();
                            _currentBuilding = null;
                        }
                    }

                    foreach (var button in _buttons.Where(b => b.IsFocused))
                        _hudMeneger.PressButton(button, ref _isPaused, ref _gameState, ref _matchState,
                            ref _currentBuilding);

                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    foreach (var button in _buttons.Where(b => b.IsPressed)) _hudMeneger.ReleaseButton(button);

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
                    if (e.wheel.y > 0) _camera.Move(CameraDerection.LEFT, ref _level);

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
            }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Run()
    {
        while (_running)
        {
            _frameStart = SDL_GetTicks();
            PollEvents();
            switch (_gameState)
            {
                case GameState.Init:
                case GameState.Lobby:
                case GameState.Menu:
                    _hudMeneger.RunManager();
                    _rendererMeneger.RunManager(ref _matchState);
                    break;
                case GameState.Play:
                    _matchState = true;
                    _hudMeneger.RunManager();
                    var updateThread = new Thread(_gameLogicManager.RunManager);
                    if (!_isPaused) updateThread.Start();
                    _rendererMeneger.RunManager(ref _currentBuilding, ref _matchState);
                    break;
                case GameState.GameOver:
                    _rendererMeneger.RunManager(ref _matchState);
                    break;
                case GameState.Exit:
                    _running = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _frameTime = SDL_GetTicks() - _frameStart;
            if (FrameDelay > _frameTime) SDL_Delay(FrameDelay - _frameTime);
        }

        Dispose();
    }
}