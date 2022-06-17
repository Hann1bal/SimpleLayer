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
    private Camera _camera;

    //Инициализация игровых объектов
    private Building? _currentBuilding;
    private EventMananager _eventManager;

    // Инициализация системных объектов
    private uint _frameStart;
    private uint _frameTime;
    private GameLogicManager _gameLogicManager;
    private NetworkManager _networkManager;
    private GameState _gameState;
    private Hud _hud;
    private HudManager _hudManager;
    private bool _isPaused;
    private bool _isShiftPressed;
    private Level _level;
    private bool _matchState;
    private List<Building> _playersBuildings = new();
    private List<Unit> _playersUnits = new();
    private IntPtr _renderer;

    //Инициализация игровых менеджеров
    private RenderManager _rendererManager;
    private bool _running = true;
    private Texture _textureManager = new();
    private TileManager _tileManager;

    // Инициализация словарей игровых объектов
    private Dictionary<int, Tile> _tiles = new();
    private IntPtr _window;

    //Инициализация событий
    private Stack<Event> _events = new();
    private Stack<Event> _recieveEvents = new();

    public Game()
    {
        InitSdl();
        InitGameObjects();
        InitGameManager();
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

        if (SDL_ttf.TTF_Init() < 0)
            Console.WriteLine($"There was an issue creating the renderer. {SDL_ttf.TTF_GetError()}");
    }

    private void InitGameObjects()
    {
        _gameState = GameState.Init;
        _hud = Hud.GetInstance("Hud", new SDL_Rect {x = 0, y = 0, h = 900, w = 1440},
            new SDL_Rect {x = 0, y = 0, w = ScreenWidth, h = ScreenHeight});
        _textureManager.LoadTexture(_renderer);
        _level = new Level();
        _camera = new Camera();
    }

    private void InitGameManager()
    {
        _hudManager = HudManager.GetInstance(ref _buttons, ref _hud, ref _gameState);
        _tileManager = TileManager.GetInstance(ref _tiles, ref _textureManager, ref _level);
        _rendererManager = RenderManager.GetInstance(ref _renderer, ref _playersBuildings,
            ref _textureManager, ref _camera, ref _level, ref _buttons, ref _hud, ref _tiles, ref _playersUnits);
        _gameLogicManager = GameLogicManager.GetInstance(ref _playersBuildings, ref _playersUnits, ref _events);
        _eventManager = EventMananager.GetInstance();
        _gameState = GameState.Menu;
        _networkManager = NetworkManager.GetInstance(ref _events, ref _recieveEvents);
        _hudManager.SetGameState(ref _gameState);
    }


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Run()
    {
        while (_running)
        {
            _frameStart = SDL_GetTicks();

            _eventManager.RunJob(ref _isPaused, ref _matchState, ref _isShiftPressed, ref _currentBuilding, ref _camera,
                ref _level, _buttons, ref _gameState, ref _gameLogicManager, ref _hudManager);

            switch (_gameState)
            {
                case GameState.Init:
                case GameState.Lobby:
                case GameState.Menu:
                    _hudManager.RunManager();
                    _rendererManager.RunManager(ref _matchState);
                    break;
                case GameState.Play:
                    _matchState = true;
                    _hudManager.RunManager();
                    _networkManager.RunManger();
                    if (!_isPaused) new Thread(_gameLogicManager.RunManager).Start();
                    _rendererManager.RunManager(ref _currentBuilding, ref _matchState);
                    break;
                case GameState.GameOver:
                    _rendererManager.RunManager(ref _matchState);
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