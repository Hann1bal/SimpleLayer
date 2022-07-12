using SimpleLayer.GameEngine.Managers;
using SimpleLayer.GameEngine.Objects.Hud;
using SimpleLayer.GameEngine.Objects.States;
using SDL2;
using SimpleLayer.GameEngine.Network.EventModels;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Templates;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.UtilComponents;

public class GameInitializer
{
    private GameTemplate _gameTemplate = new();

    public void RunInitialize(ref IntPtr window, ref IntPtr renderer, ref GameState gameState, ref Hud hud,
        ref Texture textureManager, ref Level level, ref Camera camera, ref HudManager hudManager,
        ref List<Buttons> buttons,
        ref TileManager tileManager, ref Dictionary<int, Tile> tiles,
        ref RenderManager rendererManager,
        ref List<Building> playersBuildings, ref List<Unit> playersUnits,
        ref Stack<BuildingEvent> events, ref Stack<BuildingEvent> receiveEvents, ref GameLogicManager gameLogicManager,
        ref EventMananager eventManager, ref MatchState matchState, ref NetworkManager networkManage)
    {
        InitSdl(ref window, ref renderer);
        InitGameObjects(ref renderer, ref gameState, ref hud, ref textureManager, ref level, ref camera);
        InitGameManager(ref hudManager, ref buttons, ref hud, ref gameState, ref tileManager, ref tiles,
            ref textureManager, ref level, ref rendererManager, ref renderer,
            ref playersBuildings, ref camera, ref playersUnits,
            ref events, ref receiveEvents, ref gameLogicManager,
            ref eventManager, ref matchState, ref networkManage);
    }

    private void InitSdl(ref IntPtr window, ref IntPtr renderer)
    {
        if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
            Console.WriteLine($"There was an issue initializing SDL. {SDL_GetError()}");

        window = SDL_CreateWindow(
            "Simple Test Debug" + Guid.NewGuid(),
            SDL_WINDOWPOS_CENTERED,
            SDL_WINDOWPOS_CENTERED,
            _gameTemplate.ScreenWidth,
            _gameTemplate.ScreenHeight,
            SDL_WindowFlags.SDL_WINDOW_VULKAN);

        if (window == IntPtr.Zero) Console.WriteLine($"There was an issue creating the window. {SDL_GetError()}");

        if (SDL_ttf.TTF_Init() < 0)
            Console.WriteLine($"There was an issue creating the renderer. {SDL_ttf.TTF_GetError()}");


        renderer = SDL_CreateRenderer(
            window,
            -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (renderer == IntPtr.Zero) Console.WriteLine($"There was an issue creating the renderer. {SDL_GetError()}");

        SDL_SetWindowGrab(window, SDL_bool.SDL_TRUE);

        if (SDL_ttf.TTF_Init() < 0)
            Console.WriteLine($"There was an issue creating the renderer. {SDL_ttf.TTF_GetError()}");
    }

    private void InitGameObjects(ref IntPtr renderer, ref GameState gameState, ref Hud hud,
        ref Texture textureManager, ref Level level, ref Camera camera)
    {
        gameState = GameState.Menu;
        hud = Hud.GetInstance("Hud", new SDL_Rect {x = 0, y = 0, h = 900, w = 1440},
            new SDL_Rect {x = 0, y = 0, w = _gameTemplate.ScreenWidth, h = _gameTemplate.ScreenHeight});
        textureManager.LoadTexture(renderer);
        level = new Level();
        camera = new Camera();
    }

    private void InitGameManager(ref HudManager hudManager, ref List<Buttons> buttons, ref Hud hud,
        ref GameState gameState, ref TileManager tileManager, ref Dictionary<int, Tile> tiles,
        ref Texture textureManager, ref Level level, ref RenderManager rendererManager, ref IntPtr renderer,
        ref List<Building> playersBuildings, ref Camera camera, ref List<Unit> playersUnits,
        ref Stack<BuildingEvent> events, ref Stack<BuildingEvent> receiveEvents, ref GameLogicManager gameLogicManager,
        ref EventMananager eventManager, ref MatchState matchState, ref NetworkManager networkManager)
    {
        hudManager = HudManager.GetInstance(ref buttons, ref hud, ref gameState);
        tileManager = TileManager.GetInstance(ref tiles, ref textureManager, ref level);
        rendererManager = RenderManager.GetInstance(ref renderer, ref playersBuildings,
            ref textureManager, ref camera, ref level, ref buttons, ref hud, ref tiles, ref playersUnits);
        gameLogicManager = GameLogicManager.GetInstance(ref playersBuildings, ref playersUnits, ref events,
            ref receiveEvents, ref level);
        eventManager = EventMananager.GetInstance();
        gameState = GameState.Menu;
        matchState = MatchState.Play;
        networkManager = NetworkManager.GetInstance(ref events, ref receiveEvents);
    }
}