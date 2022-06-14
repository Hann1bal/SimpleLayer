using SimpleLayer.GameEngine.UtilComponents;
using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers;

public class HudManager
{
    private static HudManager _hudManager;
    private readonly List<Buttons> _buttons;
    private Game.GameState _cState;
    private Game.GameState _gameState;
    private readonly Hud _hud;
    private int _mouseX;
    private int _mouseY;
    private Texture _texture;
    private int x, y;

    private HudManager(ref List<Buttons> buttons, ref Hud hud, ref Game.GameState gameState)
    {
        _buttons = buttons;
        _hud = hud;
        _gameState = gameState;
        _cState = gameState;
        Init();
    }

    public static HudManager GetInstance(ref List<Buttons> buttons, ref Hud hud, ref Game.GameState gameState)
    {
        if (_hudManager != null) return _hudManager;
        return _hudManager = new HudManager(ref buttons, ref hud, ref gameState);
    }


    //MENU BUTTON TOP 400px :Y 760px :X;  MIDDLE 530px :Y 830:X, Down 660px :Y 930px :X; dx 100
    private void Init()
    {
        switch (_gameState)
        {
            case Game.GameState.Init:
                break;
            //TODO Вынести все статичные данные в отдельный конфиг файл и инициализировать при загрузке игры.
            case Game.GameState.Menu:
                _buttons.Add(new Buttons("playTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 150, x = 1465, y = 480}, false));
                _buttons.Add(new Buttons("resumeTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 150, x = 1465, y = 480}, false));
                _buttons.Add(new Buttons("settingsTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 150, x = 1535, y = 635}, false));
                _buttons.Add(new Buttons("exitTextButton", new SDL_Rect {h = 90, w = 150, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 150, x = 1635, y = 795}, false));
                break;
            case Game.GameState.Play:
                _buttons.Add(new Buttons("settings", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
                    new SDL_Rect {h = 32, w = 32, x = 1820, y = 50}, false));
                _buttons.Add(new Buttons("pause", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
                    new SDL_Rect {h = 32, w = 32, x = 1770, y = 50}, false));
                var cnt = 1;
                for (var i = 0; i < 4; i++)
                for (var j = 0; j < 2; j++)
                {
                    _buttons.Add(new Buttons($"arab_{cnt}", new SDL_Rect {h = 210, w = 210, x = 0, y = 0},
                        new SDL_Rect {h = 90, w = 90, x = 1305 + i * 90, y = 825 + j * 90}, true));
                    cnt++;
                }

                break;
            case Game.GameState.GameOver:
                break;
            case Game.GameState.Lobby:
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

    private void ReInitHudForGameState()
    {
        if (_buttons.Count > 0) ClearAllButton();

        Init();
    }

//230 height 460-190
    private void SyncHud()
    {
        _hud.TextureName = _gameState switch
        {
            Game.GameState.Menu => "MenuHud",
            Game.GameState.Play => "Hud",
            Game.GameState.GameOver => "GameOver",
            Game.GameState.Lobby => "Lobby",
            Game.GameState.Init => "InitHud",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void RunManager()
    {
        Update();
        SyncHud();
        if (_cState != _gameState)
        {
            Console.WriteLine("i'am here");
            ReInitHudForGameState();
            _cState = _gameState;
        }
    }

    public void PressButton(Buttons button, ref bool gamePause, ref Game.GameState gameState, ref bool matchState,
        ref Building curent)
    {
        button.IsPressed = true;
        DoAction(button, ref gamePause, ref gameState, ref curent, ref matchState);
    }

    private void DoAction(Buttons button, ref bool gamePause, ref Game.GameState gameState, ref Building curent,
        ref bool matchState)
    {
        SDL_GetMouseState(out x, out y);
        Console.WriteLine($"{x}, {y}");
        switch (button.IsGameObject)
        {
            case true:
                curent = new Building(button.TextureName, x, y, 5000, 0, true);
                break;
            case false:
                switch (button.TextureName)
                {
                    case "settings":
                        _gameState = Game.GameState.Menu;
                        gameState = _gameState;
                        break;
                    case "pause":
                        button.TextureName = "resume";
                        gamePause = !gamePause;
                        break;
                    case "resume":
                        gamePause = !gamePause;
                        button.TextureName = "pause";
                        break;
                    case "playTextButton":
                        _gameState = Game.GameState.Play;
                        gameState = _gameState;
                        break;
                    case "exitTextButton":
                        _gameState = Game.GameState.Exit;
                        gameState = _gameState;
                        matchState = false;
                        break;
                    case "resumeTextButton":
                        _gameState = Game.GameState.Play;
                        gameState = _gameState;
                        break;
                }

                break;
        }
    }

    public void ReleaseButton(Buttons button)
    {
        button.IsPressed = false;
    }

    private void Update()
    {
        switch (_gameState)
        {
            case Game.GameState.Menu:
                foreach (var button in _buttons) CheckCollision(button);

                break;

            case Game.GameState.Play:
                foreach (var button in _buttons) CheckCollision(button);

                break;

            case Game.GameState.GameOver:
                foreach (var button in _buttons) CheckCollision(button);

                break;

            case Game.GameState.Lobby:
                foreach (var button in _buttons) CheckCollision(button);

                break;

            case Game.GameState.Init:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(_gameState), _gameState, null);
        }
    }

    private void CheckCollision(Buttons button)
    {
        SDL_GetMouseState(out _mouseX, out _mouseY);
        if (_mouseX > button.DRect.x && _mouseX < button.DRect.x + button.DRect.w && _mouseY > button.DRect.y &&
            _mouseY < button.DRect.y + button.DRect.h)
        {
            if (button.IsFocused && button.IsPressed)
                button.UpdateTextureName(Buttons.ButtonTextures.Pressed);
            else
                button.UpdateTextureName(Buttons.ButtonTextures.Focused);
        }
        else
        {
            button.UpdateTextureName(Buttons.ButtonTextures.Unfocused);
        }
    }
}