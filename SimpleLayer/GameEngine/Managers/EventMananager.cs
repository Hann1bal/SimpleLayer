using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.Hud;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers;

public class EventMananager
{
    private static EventMananager _eventMananager;
    private bool _isShiftPressed;

    private EventMananager()
    {
    }

    public static EventMananager? GetInstance()
    {
        if (_eventMananager != null) return _eventMananager;
        _eventMananager = new EventMananager();
        return _eventMananager;
    }

    public void RunJob(
        ref Building? currentBuilding, ref Camera camera,
        ref Level level, List<Buttons> buttons, ref GameState gameState, ref MatchState matchState,
        ref GameLogicManager gameLogicManager,
        ref HudManager hudManager, ref bool running, ref Time timer, ref Player player)
    {
        PollEvents(
            ref currentBuilding, ref camera,
            ref level, buttons, ref gameState, ref matchState, ref gameLogicManager,
            ref hudManager, ref running, ref timer, ref player);
    }

    private void PollEvents(
        ref Building? currentBuilding, ref Camera camera,
        ref Level level, List<Buttons> buttons, ref GameState gameState, ref MatchState matchState,
        ref GameLogicManager gameLogicManager,
        ref HudManager hudManager, ref bool running, ref Time timer, ref Player player)
    {
        while (SDL_PollEvent(out var e) == 1)
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    running = false;
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (e.button.button != 3)
                    {
                        if (currentBuilding != null && player.PlayerAttribute.Gold >=
                            currentBuilding.BuildingAttributes.BuildingCost)
                        {
                            gameLogicManager.BuildingWorker.PlaceBuilding(e.button.x + camera.CameraRect.x,
                                e.button.y + camera.CameraRect.y, ref currentBuilding, timer, ref player);
                            if (!_isShiftPressed)
                            {
                                currentBuilding.Dispose();
                                currentBuilding = null;
                            }
                        }
                    }
                    else
                    {
                        if (currentBuilding != null)
                        {
                            currentBuilding.Dispose();
                            currentBuilding = null;
                        }
                    }

                    foreach (var button in buttons.Where(b => b.ButtonAttribute.ButtonState
                                                                  is ButtonState.Focused &&
                                                              b.ButtonAttribute.EoDButtonState ==
                                                              EoDButtonState.Enabled)
                                 .ToArray())
                        hudManager.PressButton(button, ref gameState, ref matchState, ref currentBuilding, ref timer);
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    foreach (var button in buttons.Where(b =>
                                 b.ButtonAttribute.ButtonPressState == ButtonPressState.Pressed))
                        hudManager.ReleaseButton(button);
                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    switch (e.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_LEFT:
                            camera.Move(CameraDerection.LEFT);
                            break;
                        case SDL_Keycode.SDLK_RIGHT:
                            camera.Move(CameraDerection.RIGHT);
                            break;
                        case SDL_Keycode.SDLK_UP:
                            camera.Move(CameraDerection.UP);
                            break;
                        case SDL_Keycode.SDLK_DOWN:
                            camera.Move(CameraDerection.DONW);
                            break;
                        case SDL_Keycode.SDLK_LSHIFT:
                            _isShiftPressed = true;
                            break;
                    }

                    break;
                case SDL_EventType.SDL_KEYUP:
                    _isShiftPressed = e.key.keysym.sym switch
                    {
                        SDL_Keycode.SDLK_LSHIFT => false,
                        _ => _isShiftPressed
                    };

                    break;
                case SDL_EventType.SDL_MOUSEWHEEL:
                    if (e.wheel.y > 0) camera.Move(CameraDerection.LEFT);

                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    switch (e.motion.x)
                    {
                        case <= 2:
                            camera.Move(CameraDerection.LEFT);
                            break;
                        case >= 1915:
                            camera.Move(CameraDerection.RIGHT);
                            break;
                    }

                    switch (e.motion.y)
                    {
                        case <= 2:
                            camera.Move(CameraDerection.UP);
                            break;
                        case >= 1072:
                            camera.Move(CameraDerection.DONW);
                            break;
                    }

                    break;
            }
    }
}