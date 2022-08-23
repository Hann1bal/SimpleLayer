using SDL2;
using SimpleLayer.GameEngine.Containers;
using SimpleLayer.GameEngine.Managers;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Templates;
using SimpleLayer.GameEngine.UI.UIElements;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.UtilComponents;

public class GameInitializer
{
    private readonly GameTemplate _gameTemplate = new();

    public void RunInitialize(ref GameStateContainer gameStateContainer, ref ManagersContainer managersContainer,
        ref GameObjectsContainer gameObjectsContainer)
    {
        InitSdl(out window, out renderer);
        InitGameState(ref gameStateContainer);
        InitGameObjects(ref renderer, ref gameState, ref hud, ref level, ref camera);
        InitGameManager(ref managersContainer, ref  gameObjectsContainer );
    }

    private void InitSdl(out IntPtr window, out IntPtr renderer)
    {
        if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
            Console.WriteLine($"There was an issue initializing SDL. {SDL_GetError()}");

        window = SDL_CreateWindow(
            "Simple Test Debug",
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

    private void InitGameObjects(ref IntPtr renderer, ref Hud hud,
        ref Texture textureManager, ref Level level, ref Camera camera)
    {
        hud = Hud.GetInstance("Hud", new SDL_Rect {x = 0, y = 0, h = 900, w = 1440},
            new SDL_Rect {x = 0, y = 0, w = _gameTemplate.ScreenWidth, h = _gameTemplate.ScreenHeight});
        textureManager.LoadTexture(renderer);
        level = new Level();
        camera = new Camera();
    }

    private void InitGameState(ref GameStateContainer gameStateComponent)
    {
        gameStateComponent.Add("GameState", GameState.Menu);
        gameStateComponent.Add("MatchState", MatchState.Play);
    }

    private void InitGameManager(ref ManagersContainer managersContainer, ref GameObjectsContainer gameObjectsContainer)
    {
        managersContainer.Add("HudManager", HudManager.GetInstance(ref gameObjectsContainer.SystemBaseObjectsMap["buttons"] as List<Buttons>, ref hud, ref gameState));
        managersContainer.Add("TileManager", TileManager.GetInstance(ref tiles, ref textureManager, ref level));
        managersContainer.Add("TileManager", RenderManager.GetInstance(ref renderer, ref playersBuildings,
            ref textureManager, ref camera, ref level, ref buttons, ref hud, ref tiles, ref playersUnits,
            ref textInput));
        managersContainer.Add("GameLogicManager", GameLogicManager.GetInstance(ref playersBuildings, ref playersUnits,
            ref events,
            ref receiveEvents, ref level));
        managersContainer.Add("EventMananager", EventMananager.GetInstance());
        managersContainer.Add("NetworkManager", NetworkManager.GetInstance(ref events, ref receiveEvents));
    }
}