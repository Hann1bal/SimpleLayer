using System.Text;
using SimpleLayer.GameEngine.Containers;
using SimpleLayer.GameEngine.Managers.Workers;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UI.UIStates;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers;

public class EventMananager:IBaseManger
{
    private static EventMananager _eventMananager;
    private bool _isShiftPressed;
    private EventWorker _eventWorker = new EventWorker();
    private EventType _eventType;
    public string ManagerName { get; set; }

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
        ref HudManager hudManager, ref bool running, ref Time timer, ref Player player, ref TextInput textInput)
    {
        PollEvents(
            ref currentBuilding, ref camera,
            ref level, buttons, ref gameState, ref matchState, ref gameLogicManager,
            ref hudManager, ref running, ref timer, ref player, ref textInput);
    }

    //TODO Переделать обработчик событий в более компактный вид, добавить состояния для корректного упралвления ввода и избежания коллизий.
    private void PollEvents(
        ref Building? currentBuilding, ref Camera camera,
        ref Level level, List<Buttons> buttons, ref GameState gameState, ref MatchState matchState,
        ref GameLogicManager gameLogicManager,
        ref HudManager hudManager, ref bool running, ref Time timer, ref Player player, ref TextInput textInput)
    {
        if (SDL_IsTextInputActive() != SDL_bool.SDL_FALSE && textInput.TextInputStates == TextInputStates.Unfocused)
            SDL_StopTextInput();
        while (SDL_PollEvent(out var e) == 1)
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    running = false;
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (e.button.button != 3)
                    {
                        if (currentBuilding != null &&
                            currentBuilding.BuildingAttributes.BuildingPlaceState == BuildingPlaceState.NonPlaced &&
                            player.PlayerAttribute.Gold >=
                            currentBuilding.BuildingAttributes.BuildingCost)
                        {
                            _eventType = EventType.PlaseBuilding;
                            _eventWorker.RunJob(ref _eventType, ref gameLogicManager, ref e, ref camera,
                                ref currentBuilding, ref timer, ref player, ref _isShiftPressed, ref buttons,
                                ref hudManager, ref gameState, ref matchState);
                            return;
                        }

                        if (gameLogicManager.BuildingWorker.BuildingFoccused(e.button.x + camera.CameraRect.x,
                                e.button.y + camera.CameraRect.y))
                        {
                            _eventType = EventType.SelectBuilding;
                            _eventWorker.RunJob(ref _eventType, ref gameLogicManager, ref e, ref camera,
                                ref currentBuilding, ref timer, ref player, ref _isShiftPressed, ref buttons,
                                ref hudManager, ref gameState, ref matchState);
                        }
                    }
                    else
                    {
                        if (currentBuilding != null && currentBuilding.BuildingAttributes.BuildingPlaceState ==
                            BuildingPlaceState.NonPlaced)
                        {
                            _eventType = EventType.UnselectTemplate;
                            _eventWorker.RunJob(ref _eventType, ref gameLogicManager, ref e, ref camera,
                                ref currentBuilding, ref timer, ref player, ref _isShiftPressed, ref buttons,
                                ref hudManager, ref gameState, ref matchState);
                        }
                        else if (currentBuilding != null && currentBuilding.BuildingAttributes.BuildingPlaceState ==
                                 BuildingPlaceState.Selected)
                        {
                            _eventType = EventType.UnselectCurrentBuilding;
                            _eventWorker.RunJob(ref _eventType, ref gameLogicManager, ref e, ref camera,
                                ref currentBuilding, ref timer, ref player, ref _isShiftPressed, ref buttons,
                                ref hudManager, ref gameState, ref matchState);
                        }
                    }

                    if (buttons.Any(b => b.ButtonAttribute.ButtonState
                                               is ButtonState.Focused &&
                                           b.ButtonAttribute.EoDButtonState ==
                                           EoDButtonState.Enabled))
                    {
                        _eventType = EventType.PressButton;
                        _eventWorker.RunJob(ref _eventType, ref gameLogicManager, ref e, ref camera,
                            ref currentBuilding, ref timer, ref player, ref _isShiftPressed, ref buttons,
                            ref hudManager, ref gameState, ref matchState);
                    }

                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    _eventType = EventType.ReleaseButton;
                    _eventWorker.RunJob(ref _eventType, ref gameLogicManager, ref e, ref camera,
                        ref currentBuilding, ref timer, ref player, ref _isShiftPressed, ref buttons, ref hudManager,
                        ref gameState, ref matchState);
                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    switch (e.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_LEFT:
                            camera.Move(CameraDirectionState.Left);
                            break;
                        case SDL_Keycode.SDLK_RIGHT:
                            camera.Move(CameraDirectionState.Right);
                            break;
                        case SDL_Keycode.SDLK_UP:
                            camera.Move(CameraDirectionState.Up);
                            break;
                        case SDL_Keycode.SDLK_DOWN:
                            camera.Move(CameraDirectionState.Donw);
                            break;
                        case SDL_Keycode.SDLK_LSHIFT:
                            _isShiftPressed = true;
                            break;
                        case SDL_Keycode.SDLK_TAB:
                            textInput.TextInputStates = TextInputStates.Focused;
                            SDL_SetTextInputRect(ref textInput.TextInputRec);
                            SDL_StartTextInput();

                            break;
                        case SDL_Keycode.SDLK_RETURN:
                            if (textInput.TextInputStates == TextInputStates.Focused && !_isShiftPressed)
                            {
                                //TextHandler 
                                textInput.ClearBufferString();
                                SDL_StopTextInput();
                                textInput.TextInputStates = TextInputStates.Unfocused;
                            }

                            break;
                        case SDL_Keycode.SDLK_BACKSPACE:
                            if (textInput.TextInputStates == TextInputStates.Focused && textInput.Textbuffer.Length > 0)
                                textInput.Textbuffer = textInput.Textbuffer.Remove(textInput.Textbuffer.Length - 1);

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

                case SDL_EventType.SDL_TEXTINPUT:
                    unsafe
                    {
                        if (textInput.TextInputRec.w >= textInput.MaxLenght)
                        {
                            textInput.LastString = textInput.Textbuffer;
                            textInput.Textbuffer += Encoding.UTF8.GetString(e.text.text, 1);
                            textInput.CurentString = textInput.Textbuffer;
                            textInput.flag = true;
                        }
                        else
                        {
                            textInput.Textbuffer += Encoding.UTF8.GetString(e.text.text, 1);
                        }
                    }

                    break;

                case SDL_EventType.SDL_MOUSEWHEEL:
                    if (e.wheel.y > 0) camera.Move(CameraDirectionState.Left);

                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    Console.WriteLine($"x: {e.motion.x}, y:{e.motion.y}");
                    switch (e.motion.x)
                    {
                        case <= 2:
                            camera.Move(CameraDirectionState.Left);
                            break;
                        case >= 1915:
                            camera.Move(CameraDirectionState.Right);
                            break;
                    }

                    switch (e.motion.y)
                    {
                        case <= 2:
                            camera.Move(CameraDirectionState.Up);
                            break;
                        case >= 1072:
                            camera.Move(CameraDirectionState.Donw);
                            break;
                    }

                    break;
            }
    }
}