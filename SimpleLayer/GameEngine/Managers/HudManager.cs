using SimpleLayer.GameEngine.Containers;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers;

public class HudManager:IBaseManger
{
    private static HudManager _hudManager;
    private readonly List<Buttons> _buttons;
    private readonly Hud _hud;
    private int _mouseX;
    private int _mouseY;
    private Texture _texture;
    private GameState curentGameState;
    private int x, y;

    private HudManager(ref List<Buttons> buttons, ref Hud hud, ref GameState gameState)
    {
        _buttons = buttons;
        _hud = hud;
        _hud.hudBaseObjectAttribute.GameObjectsLowerButtonsCol = 4;
        _hud.hudBaseObjectAttribute.GameObjectsLowerButtonsRow = 2;
        Init();
    }

    public GameBaseObject? CurrentObject { get; set; }

    public static HudManager GetInstance(ref List<Buttons> buttons, ref Hud hud, ref GameState gameState)
    {
        if (_hudManager != null) return _hudManager;
        return _hudManager = new HudManager(ref buttons, ref hud, ref gameState);
    }

    //MENU BUTTON TOP 400px :Y 760px :X;  MIDDLE 530px :Y 830:X, Down 660px :Y 930px :X; dx 100
    private void Init()
    {
        _buttons.Add(new Buttons("playTextButton", new SDL_Rect {h = 100, w = 150, x = 0, y = 0},
            new SDL_Rect {h = 90, w = 150, x = 1465, y = 480}, ButtonType.MenuButton, null, EoDButtonState.Enabled));
        _buttons.Add(new Buttons("resumeTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
            new SDL_Rect {h = 90, w = 150, x = 1465, y = 480}, ButtonType.MenuButton, null, EoDButtonState.Disabled));
        _buttons.Add(new Buttons("settingsTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
            new SDL_Rect {h = 90, w = 150, x = 1535, y = 635}, ButtonType.MenuButton, null, EoDButtonState.Enabled));
        _buttons.Add(new Buttons("exitTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
            new SDL_Rect {h = 90, w = 150, x = 1635, y = 795}, ButtonType.MenuButton, null, EoDButtonState.Enabled));

        _buttons.Add(new Buttons("settings", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
            new SDL_Rect {h = 32, w = 32, x = 1820, y = 50}, ButtonType.Blank, null, EoDButtonState.Enabled));
        _buttons.Add(new Buttons("pause", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
            new SDL_Rect {h = 32, w = 32, x = 1770, y = 50}, ButtonType.Blank, null, EoDButtonState.Enabled));
        _buttons.Add(new Buttons("resume", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
            new SDL_Rect {h = 32, w = 32, x = 1770, y = 50}, ButtonType.Blank, null, EoDButtonState.Disabled));
        _buttons.Add(new Buttons("upgradeTextButton", new SDL_Rect {h = 100, w = 150, x = 0, y = 0},
            new SDL_Rect {h = 90, w = 150, x = 500, y = 900}, ButtonType.MatchHudButton, null,
            EoDButtonState.Disabled));
        _buttons.Add(new Buttons("destroyTextButton", new SDL_Rect {h = 100, w = 150, x = 0, y = 0},
            new SDL_Rect {h = 90, w = 150, x = 500, y = 1000}, ButtonType.MatchHudButton, null,
            EoDButtonState.Disabled));
        var cnt = 1;
        for (var i = 0; i < _hud.hudBaseObjectAttribute.GameObjectsLowerButtonsCol; i++)
        for (var j = 0; j < _hud.hudBaseObjectAttribute.GameObjectsLowerButtonsRow; j++)
        {
            _buttons.Add(new Buttons($"arab_{cnt}", new SDL_Rect {h = 210, w = 210, x = 0, y = 0},
                new SDL_Rect {h = 90, w = 90, x = 1305 + i * 90, y = 825 + j * 90}, ButtonType.MatchBuildingButton,
                (GameObjectsButtonType?) cnt, EoDButtonState.Enabled));
            cnt++;
        }
    }

    public void RunManager(ref GameState gameState, ref MatchState matchState)
    {
        Update(ref gameState, ref matchState);
        SyncHud(ref gameState);
        curentGameState = gameState;
    }

    private void DoAction(Buttons button, ref GameState gameState, ref MatchState matchState, ref Building? current,
        ref Time timer)
    {
        SDL_GetMouseState(out x, out y);
        switch (button.ButtonAttribute.ButtonType)
        {
            case ButtonType.MatchBuildingButton:
                current = new Building(button.hudBaseObjectAttribute.TextureName, x, y, 100, 0, timer.Seconds,
                    BuildingType.Factory, BuildingPlaceState.NonPlaced, 210, 210);
                break;
            case ButtonType.MatchHudButton:
                switch (button.hudBaseObjectAttribute.TextureName)
                {
                    case "upgradeTextButton":
                        if (current.BaseObjectAttribute.Tier < 4) current.BaseObjectAttribute.Tier++;
                        break;
                    case "destroyTextButton":

                        current.BaseObjectAttribute.DoAState = DoAState.Dead;
                        current = null;
                        break;
                }

                break;
            case ButtonType.MenuButton:
                switch (button.hudBaseObjectAttribute.TextureName)
                {
                    case "playTextButton":
                        gameState = GameState.Match;
                        matchState = MatchState.Play;
                        button.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                        foreach (var btns in _buttons.Where(btb =>
                                     btb.ButtonAttribute.ButtonType != ButtonType.MenuButton &&
                                     btb.ButtonAttribute.ButtonType != ButtonType.Setting))
                            switch (matchState)
                            {
                                case MatchState.Play when btns.hudBaseObjectAttribute.TextureName == "resume":
                                case MatchState.Pause when btns.hudBaseObjectAttribute.TextureName == "pause":
                                    continue;
                                default:
                                    if (btns.ButtonAttribute.ButtonType == ButtonType.MatchHudButton && current == null)
                                        btns.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                                    else btns.ButtonAttribute.EoDButtonState = EoDButtonState.Enabled;
                                    break;
                            }

                        _buttons.First(btn => btn.hudBaseObjectAttribute.TextureName == "resumeTextButton")
                            .ButtonAttribute.EoDButtonState = EoDButtonState.Enabled;
                        break;
                    case "exitTextButton":
                        gameState = GameState.Exit;
                        break;
                    case "resumeTextButton":
                        gameState = GameState.Match;
                        // button.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                        foreach (var btns in _buttons.Where(btb =>
                                     btb.ButtonAttribute.ButtonType != ButtonType.MenuButton &&
                                     btb.ButtonAttribute.ButtonType != ButtonType.Setting))
                            switch (matchState)
                            {
                                case MatchState.Play when btns.hudBaseObjectAttribute.TextureName == "resume":
                                case MatchState.Pause when btns.hudBaseObjectAttribute.TextureName == "pause":
                                    continue;
                                default:
                                    btns.ButtonAttribute.EoDButtonState = EoDButtonState.Enabled;
                                    break;
                            }

                        break;
                    case "settingsTextButton":
                        gameState = GameState.Settings;

                        break;
                }

                break;
            case ButtonType.Blank:
                switch (button.hudBaseObjectAttribute.TextureName)
                {
                    case "settings":
                        button.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                        foreach (var btns in _buttons.Where(btb =>
                                     btb.ButtonAttribute.ButtonType != ButtonType.MenuButton &&
                                     btb.ButtonAttribute.ButtonType != ButtonType.Setting))
                            btns.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                        gameState = GameState.MatchPauseMenu;
                        break;
                    case "pause":
                        button.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                        _buttons.First(btn => btn.hudBaseObjectAttribute.TextureName == "resume")
                            .ButtonAttribute.EoDButtonState = EoDButtonState.Enabled;
                        matchState = MatchState.Pause;
                        break;
                    case "resume":
                        button.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                        _buttons.First(btn => btn.hudBaseObjectAttribute.TextureName == "pause")
                            .ButtonAttribute.EoDButtonState = EoDButtonState.Enabled;
                        matchState = MatchState.Play;
                        break;
                }

                break;
            case ButtonType.Setting:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void PressButton(Buttons button, ref GameState gameState, ref MatchState matchState, ref Building? current,
        ref Time timer)
    {
        button.ButtonAttribute.ButtonPressState = ButtonPressState.Pressed;
        DoAction(button, ref gameState, ref matchState, ref current, ref timer);
    }

    public void ReleaseButton(Buttons button, ref Building? current)
    {
        button.ButtonAttribute.ButtonPressState = ButtonPressState.Released;
        if (current == null)
            foreach (var btns in _buttons
                         .Where(c => c.ButtonAttribute.ButtonType == ButtonType.MatchHudButton)
                         .ToList())
            {
                btns.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                btns.ButtonAttribute.ButtonPressState = ButtonPressState.Released;
                btns.ButtonAttribute.ButtonState = ButtonState.Unfocused;
            }
    }

    //230 height 460-190
    private void SyncHud(ref GameState gameState)
    {
        _hud.hudBaseObjectAttribute.CurrentTextureName = gameState switch
        {
            GameState.Menu => "MenuHud",
            GameState.MatchPauseMenu => "MenuHud",
            GameState.Match => "Hud",
            GameState.GameOver => "GameOver",
            GameState.Lobby => "Lobby",
            GameState.Init => "InitHud",
            _ => throw new ArgumentOutOfRangeException()
        };
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

    private void CheckBuildingCollision()
    {
        SDL_GetMouseState(out _mouseX, out _mouseY);
    }

    private void Update(ref GameState gameState, ref MatchState matchState)
    {
        switch (gameState)
        {
            case GameState.MatchPauseMenu:
            case GameState.Menu:
                foreach (var button in _buttons.Where(button =>
                             button.ButtonAttribute.ButtonType is ButtonType.MenuButton &&
                             button.ButtonAttribute.EoDButtonState == EoDButtonState.Enabled))
                    CheckButtonCollision(button);
                break;
            case GameState.Match:
                switch (matchState)
                {
                    case MatchState.Play:
                        foreach (var button in _buttons.Where(button =>
                                     button.ButtonAttribute.ButtonType is ButtonType.MatchBuildingButton
                                         or ButtonType.Blank or ButtonType.MatchHudButton &&
                                     button.ButtonAttribute.EoDButtonState == EoDButtonState.Enabled))
                            CheckButtonCollision(button);
                        break;

                    case MatchState.Pause:
                        foreach (var button in _buttons.Where(button =>
                                     button.ButtonAttribute.ButtonType is ButtonType.MatchBuildingButton
                                         or ButtonType.Blank or ButtonType.MatchHudButton &&
                                     button.ButtonAttribute.EoDButtonState == EoDButtonState.Enabled))
                            CheckButtonCollision(button);
                        break;
                }

                break;

            case GameState.GameOver:
                foreach (var button in _buttons) CheckButtonCollision(button);
                break;

            case GameState.Lobby:
                foreach (var button in _buttons) CheckButtonCollision(button);
                break;

            case GameState.Init:
            case GameState.Exit:
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