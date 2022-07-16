using System.Text;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UI.UIStates;
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
                            if (textInput.TextInputStates == TextInputStates.Focused)
                            {
                                if (textInput.TextInputRec.x < 0) textInput.TextInputRec.x += 20;
                            }
                            else
                            {
                                camera.Move(CameraDirectionState.Left);
                            }

                            break;
                        case SDL_Keycode.SDLK_RIGHT:
                            if (textInput.TextInputStates == TextInputStates.Focused)
                            {
                                if (textInput.Textbuffer.Length * 20 + textInput.TextInputRec.x != 485)
                                {
                                    textInput.TextInputRec.x -= 20;
                                }
                            }
                            else
                            {
                                camera.Move(CameraDirectionState.Right);
                            }

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
                            {
                                textInput.Textbuffer = textInput.Textbuffer.Remove(textInput.Textbuffer.Length - 1);
                            }

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
                        if (textInput.TextInputRec.w >= textInput.MaxLenght &&
                            textInput.TextInputRec.w > textInput.CurentLenght)
                        {
                            textInput.TextInputRec.x -= 20;
                            textInput.CurentLenght = textInput.TextInputRec.w;
                            textInput.Textbuffer += Encoding.UTF8.GetString(e.text.text, 1);
                            Console.WriteLine(Encoding.UTF8.GetString(e.text.text, 1));
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