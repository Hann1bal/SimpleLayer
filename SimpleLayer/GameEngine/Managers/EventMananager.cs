using SDL2;
using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers;

public class EventMananager
{
    private static EventMananager _eventMananager;

    private EventMananager()
    {
    }

    public static EventMananager GetInstance()
    {
        if (_eventMananager != null) return _eventMananager;
        _eventMananager = new EventMananager();
        return _eventMananager;
    }

    public void RunJob(ref bool isPaused, ref bool matchState, ref bool isShiftPressed,
        ref Building currentBuilding, ref Camera camera,
        ref Level level, List<Buttons> buttons, ref Game.GameState gameState, ref GameLogicManager gameLogicManager,
        ref HudManager hudManager)
    {
        PollEvents(ref isPaused, ref matchState, ref isShiftPressed,
            ref currentBuilding, ref camera,
            ref level, buttons, ref gameState, ref gameLogicManager,
            ref hudManager);
    }

    private void PollEvents(ref bool isPaused, ref bool matchState, ref bool isShiftPressed,
        ref Building currentBuilding, ref Camera camera,
        ref Level level, List<Buttons> buttons, ref Game.GameState gameState, ref GameLogicManager gameLogicManager,
        ref HudManager hudManager)
    {
        while (SDL_PollEvent(out var e) == 1)
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    isShiftPressed = false;
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (e.button.button != 3)
                    {
                        if (currentBuilding != null)
                        {
                            gameLogicManager.BuildingWorker.PlaceBuilding(e.button.x + camera.CameraRect.x,
                                e.button.y + camera.CameraRect.y, ref currentBuilding, ref level);
                            if (!isShiftPressed)
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

                    foreach (var button in buttons.Where(b => b.IsFocused))
                        hudManager.PressButton(button, ref isPaused, ref gameState, ref matchState,
                            ref currentBuilding);

                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    foreach (var button in buttons.Where(b => b.IsPressed)) hudManager.ReleaseButton(button);

                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    switch (e.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_LEFT:
                            camera.Move(CameraDerection.LEFT, ref level);
                            break;
                        case SDL_Keycode.SDLK_RIGHT:
                            camera.Move(CameraDerection.RIGHT, ref level);
                            break;
                        case SDL_Keycode.SDLK_UP:
                            camera.Move(CameraDerection.UP, ref level);
                            break;
                        case SDL_Keycode.SDLK_DOWN:
                            camera.Move(CameraDerection.DONW, ref level);
                            break;
                        case SDL_Keycode.SDLK_LSHIFT:
                            isShiftPressed = true;
                            break;
                    }

                    break;
                case SDL_EventType.SDL_KEYUP:
                    isShiftPressed = e.key.keysym.sym switch
                    {
                        SDL_Keycode.SDLK_LSHIFT => false,
                        _ => isShiftPressed
                    };

                    break;
                case SDL_EventType.SDL_MOUSEWHEEL:
                    if (e.wheel.y > 0) camera.Move(CameraDerection.LEFT, ref level);

                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    switch (e.motion.x)
                    {
                        case <= 2:
                            camera.Move(CameraDerection.LEFT, ref level);
                            break;
                        case >= 1915:
                            camera.Move(CameraDerection.RIGHT, ref level);
                            break;
                    }

                    switch (e.motion.y)
                    {
                        case <= 2:
                            camera.Move(CameraDerection.UP, ref level);
                            break;
                        case >= 1072:
                            camera.Move(CameraDerection.DONW, ref level);
                            break;
                    }

                    break;
            }
    }
}