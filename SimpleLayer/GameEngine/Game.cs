using SimpleLayer.GameEngine.Containers;
using SimpleLayer.GameEngine.Managers;
using SimpleLayer.GameEngine.Network.EventModels;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.UI.UIAttributes;
using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class Game : IDisposable
{
    // Инициализация констант графического движка

    private const int Fps = 30;
    private const int FrameDelay = 1000 / Fps;

    // Инициализация списков игровых объектов
    private readonly List<Buttons> _buttons = new();
    private Camera _camera;

    //Инициализация игровых объектов
    private Building? _currentBuilding;
    private readonly EventMananager? _eventManager;

    //Инициализация событий
    private readonly Stack<BuildingEvent> _events = new();

    // Инициализация системных объектов
    private uint _frameStart;
    private uint _frameTime;

    //Инициализация игрового таймера
    private uint _gameClock;
    private GameLogicManager _gameLogicManager;
    private GameState _gameState;
    private readonly Hud _hud;
    private HudManager _hudManager;
    private bool _isPaused;
    private Level _level;
    private MatchState _matchState;
    private readonly NetworkManager _networkManager;
    private Player _player = new();
    private readonly List<Building> _playersBuildings = new();
    private readonly List<Unit> _playersUnits = new();
    private readonly Stack<BuildingEvent> _receiveEvents = new();
    private readonly IntPtr _renderer;

    //Инициализация игровых менеджеров
    private readonly RenderManager _rendererManager;
    private bool _running = true;

    private TextInput _textInput = new(new TextInputAttribute
        {XStartPos = 5, SizeAxisX = 20, YStartPos = 600, SizeAxisY = 40});

    private readonly Texture _textureManager = new();
    private readonly TileManager _tileManager;

    // Инициализация словарей игровых объектов
    private readonly Dictionary<int, Tile> _tiles = new();
    private readonly IntPtr _window;
    private Time Timer = new();

    // Инициализация контейнеров
    private GameStateContainer _gameStateContainer = new GameStateContainer();
    private ManagersContainer _managersContainer = new ManagersContainer();
    /*
     ref _window, ref _renderer, ref _gameState, ref _hud, ref _textureManager,
            ref _level, ref _camera, ref _hudManager, ref _buttons, ref _tileManager, ref _tiles, ref _rendererManager,
            ref _playersBuildings, ref _playersUnits, ref _events, ref _receiveEvents, ref _gameLogicManager,
            ref _eventManager, ref _matchState, ref _networkManager, ref _textInput
            */
    public Game()
    {
        var gameInitializer = new GameInitializer();
        gameInitializer.RunInitialize(ref _managersContainer, ref _gameStateContainer);
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
                ref _running, ref Timer, ref _player, ref _textInput);

            switch (_gameState)
            {
                case GameState.Init:
                case GameState.Lobby:
                    break;
                case GameState.Settings:
                    _hudManager.RunManager(ref _gameState, ref _matchState);
                    _rendererManager.RunManager();
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