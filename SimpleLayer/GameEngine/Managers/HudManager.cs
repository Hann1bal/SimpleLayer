using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.UtilComponents;
using SimpleLayer.Objects;
using SimpleLayer.Objects.States;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers;

public class HudManager
{
    private static HudManager _hudManager;
    private readonly List<Buttons> _buttons;
    private readonly Hud _hud;
    private int _mouseX;
    private int _mouseY;
    private Texture _texture;
    private Game.GameState curentGameState;
    private int x, y;

    private HudManager(ref List<Buttons> buttons, ref Hud hud, ref Game.GameState gameState)
    {
        _buttons = buttons;
        _hud = hud;
        Init(ref gameState);
    }

    public static HudManager GetInstance(ref List<Buttons> buttons, ref Hud hud, ref Game.GameState gameState)
    {
        if (_hudManager != null) return _hudManager;
        return _hudManager = new HudManager(ref buttons, ref hud, ref gameState);
    }

    //MENU BUTTON TOP 400px :Y 760px :X;  MIDDLE 530px :Y 830:X, Down 660px :Y 930px :X; dx 100
    private void Init(ref Game.GameState gameState)
    {
        switch (gameState)
        {
            case Game.GameState.Init:
                break;
            //TODO Вынести все статичные данные в отдельный конфиг файл и инициализировать при загрузке игры.
            case Game.GameState.Menu:
                _buttons.Add(new Buttons("playTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 150, x = 1465, y = 480}, ButtonType.MenuButton));
                _buttons.Add(new Buttons("resumeTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 150, x = 1465, y = 480}, ButtonType.MenuButton));
                _buttons.Add(new Buttons("settingsTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 150, x = 1535, y = 635}, ButtonType.MenuButton));
                _buttons.Add(new Buttons("exitTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 150, x = 1635, y = 795}, ButtonType.MenuButton));
                break;
            case Game.GameState.Play:
                _buttons.Add(new Buttons("settings", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
                    new SDL_Rect {h = 32, w = 32, x = 1820, y = 50}, ButtonType.Blank));
                _buttons.Add(new Buttons("pause", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
                    new SDL_Rect {h = 32, w = 32, x = 1770, y = 50}, ButtonType.Blank));
                var cnt = 1;
                for (var i = 0; i < 4; i++)
                for (var j = 0; j < 2; j++)
                {
                    _buttons.Add(new Buttons($"arab_{cnt}", new SDL_Rect {h = 210, w = 210, x = 0, y = 0},
                        new SDL_Rect {h = 90, w = 90, x = 1305 + i * 90, y = 825 + j * 90}, ButtonType.MatchHudButton));
                    cnt++;
                }

                break;
            case Game.GameState.GameOver:
                break;
            case Game.GameState.Lobby:
                break;
            case Game.GameState.Exit:
                break;
            case Game.GameState.Pause:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RunManager(ref Game.GameState gameState)
    {
        Update(ref gameState);
        SyncHud(ref gameState);
        if (gameState != curentGameState)
        {
            ReInitHudForGameState(ref gameState);
            curentGameState = gameState;
        }
    }

    private void DoAction(Buttons button, ref bool gamePause, ref Game.GameState gameState, ref Building? current,
        ref bool matchState, ref Time timer)
    {
        SDL_GetMouseState(out x, out y);
        switch (button.ButtonAttribute.ButtonType)
        {
            case ButtonType.MatchHudButton:
                current = new Building(button.hudBaseObjectAttribute.TextureName, x, y, 100, 0, timer.Seconds,
                    BuildingType.Factory);
                break;
            case ButtonType.MenuButton:
                switch (button.hudBaseObjectAttribute.TextureName)
                {
                    case "playTextButton":
                        gameState = Game.GameState.Play;
                        break;
                    case "exitTextButton":
                        gameState = Game.GameState.Exit;
                        matchState = false;
                        break;
                    case "resumeTextButton":
                        gameState = Game.GameState.Play;
                        break;
                }

                break;
            case ButtonType.Blank:
                switch (button.hudBaseObjectAttribute.CurrentTextureName)
                {
                    case "settings":
                        gameState = Game.GameState.Menu;
                        break;
                    case "pause":
                        button.hudBaseObjectAttribute.CurrentTextureName = "resume";
                        gamePause = !gamePause;
                        gameState = Game.GameState.Pause;
                        break;
                    case "resume":
                        button.hudBaseObjectAttribute.CurrentTextureName = "pause";
                        gamePause = !gamePause;
                        gameState = Game.GameState.Play;
                        break;
                }

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void PressButton(Buttons button, ref bool gamePause, ref Game.GameState gameState, ref bool matchState,
        ref Building? current, ref Time timer)
    {
        button.ButtonAttribute.ButtonPressState = ButtonPressState.Pressed;
        DoAction(button, ref gamePause, ref gameState, ref current, ref matchState, ref timer);
    }

    public void ReleaseButton(Buttons button)
    {
        button.ButtonAttribute.ButtonPressState = ButtonPressState.Released;
    }

    //230 height 460-190
    private void SyncHud(ref Game.GameState gameState)
    {
        _hud.hudBaseObjectAttribute.CurrentTextureName = gameState switch
        {
            Game.GameState.Menu => "MenuHud",
            Game.GameState.Play => "Hud",
            Game.GameState.GameOver => "GameOver",
            Game.GameState.Lobby => "Lobby",
            Game.GameState.Init => "InitHud",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void ReInitHudForGameState(ref Game.GameState gameState)
    {
        if (_buttons.Count > 0) ClearAllButton();

        Init(ref gameState);
    }

    private void CheckButtonCollision(Buttons button)
    {
        SDL_GetMouseState(out _mouseX, out _mouseY);
        if (_mouseX > button.DRect.x && _mouseX < button.DRect.x + button.DRect.w && _mouseY > button.DRect.y &&
            _mouseY < button.DRect.y + button.DRect.h)
        {
            if (button.ButtonAttribute.ButtonState == ButtonState.Focused &&
                button.ButtonAttribute.ButtonPressState == ButtonPressState.Pressed)
                button.UpdateTextureName(ButtonState.Focused, ButtonPressState.Pressed);
            else button.UpdateTextureName(ButtonState.Focused, ButtonPressState.Released);
        }
        else
        {
            button.UpdateTextureName(ButtonState.Unfocused, ButtonPressState.Released);
        }
    }

    private void Update(ref Game.GameState gameState)
    {
        switch (gameState)
        {
            case Game.GameState.Menu:
                foreach (var button in _buttons) CheckButtonCollision(button);
                break;

            case Game.GameState.Play:
                foreach (var button in _buttons) CheckButtonCollision(button);
                break;

            case Game.GameState.GameOver:
                foreach (var button in _buttons) CheckButtonCollision(button);
                break;

            case Game.GameState.Lobby:
                foreach (var button in _buttons) CheckButtonCollision(button);
                break;

            case Game.GameState.Init:
                break;

            case Game.GameState.Exit:
            case Game.GameState.Pause:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void ClearAllButton()
    {
        foreach (var button in _buttons.ToArray())
        {
            _buttons.Remove(button);
            button.Dispose();
        }
    }
}