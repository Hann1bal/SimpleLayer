using SDL2;
using SimpleLayer.GameEngine.Managers;
using SimpleLayer.GameEngine.Network.EventModels;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.Hud;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class Game : IDisposable
{
    // Инициализация констант графического движка

    private const int Fps = 30;
    private const int FrameDelay = 1000 / Fps;

    // Инициализация списков игровых объектов
    private List<Buttons> _buttons = new();
    private Camera _camera;

    //Инициализация игровых объектов
    private Building? _currentBuilding;
    private EventMananager? _eventManager;

    //Инициализация событий
    private Stack<BuildingEvent> _events = new();

    //Инициализация игрового таймера
    private uint _gameClock;

    // Инициализация системных объектов
    private uint _frameStart;
    private uint _frameTime;
    private GameLogicManager _gameLogicManager;
    private GameState _gameState;
    private Hud _hud;
    private bool _isPaused;
    private Level _level;
    private MatchState _matchState;
    private Player _player = new();
    private List<Building> _playersBuildings = new();
    private List<Unit> _playersUnits = new();
    private Stack<BuildingEvent> _receiveEvents = new();
    private IntPtr _renderer;


    //Инициализация игровых менеджеров
    private RenderManager _rendererManager;
    private bool _running = true;
    private Texture _textureManager = new();
    private TileManager _tileManager;
    private NetworkManager _networkManager;
    private HudManager _hudManager;

    // Инициализация словарей игровых объектов
    private Dictionary<int, Tile> _tiles = new();
    private IntPtr _window;
    private Time Timer = new();

    public Game()
    {
        GameInitializer gameInitializer = new GameInitializer();
        gameInitializer.RunInitialize(ref _window, ref _renderer, ref _gameState, ref _hud, ref _textureManager,
            ref _level, ref _camera, ref _hudManager, ref _buttons, ref _tileManager, ref _tiles, ref _rendererManager,
            ref _playersBuildings, ref _playersUnits, ref _events, ref _receiveEvents, ref _gameLogicManager,
            ref _eventManager, ref _matchState, ref _networkManager);
    }

    public void Dispose()
    {
        _networkManager.Disconnect();
        _textureManager.ClearAllTexture();
        SDL_DestroyRenderer(_renderer);
        SDL_DestroyWindow(_window);
        SDL_Quit();
        GC.SuppressFinalize(this);
        GC.Collect();
    }


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Run()
    {
        while (_running)
        {
            _frameStart = SDL_GetTicks();

            _eventManager.RunJob(ref _currentBuilding, ref _camera,
                ref _level, _buttons, ref _gameState, ref _matchState, ref _gameLogicManager, ref _hudManager,
                ref _running, ref Timer, ref _player);

            switch (_gameState)
            {
                case GameState.Init:
                case GameState.Lobby:
                    break;
                case GameState.MatchPauseMenu:
                case GameState.Menu:
                    _hudManager.RunManager(ref _gameState, ref _matchState);
                    _rendererManager.RunManager();
                    break;
                case GameState.Match:
                    switch (_matchState)
                    {
                        case MatchState.Play:
                            _hudManager.RunManager(ref _gameState, ref _matchState);
                            // _networkManager.RunManger();
                            object dos = new object[] {Timer, _player};
                            if (!_isPaused) new Thread(_gameLogicManager.RunManager).Start(dos);
                            _rendererManager.RunManager(ref _currentBuilding, ref Timer.Seconds,
                                ref _player);
                            Timer.StartGameTimer();
                            break;
                        case MatchState.Pause:
                            _hudManager.RunManager(ref _gameState, ref _matchState);
                            // _networkManager.RunManger();
                            _rendererManager.RunManager(ref _currentBuilding, ref Timer.Seconds,
                                ref _player);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case GameState.GameOver:
                    _rendererManager.RunManager();
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