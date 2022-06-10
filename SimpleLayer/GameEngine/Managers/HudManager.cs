using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers;

public class HudManager
{
    private static HudManager _hudManager = null;
    private List<Buttons> _buttons;
    private int _mouseX;
    private int _mouseY;
    private int x, y;

    private HudManager(ref List<Buttons> buttons, ref Hud hud)
    {
        _buttons = buttons;
        Init();
    }

    private void Init()
    {
        _buttons.Add(new Buttons("settings", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
            new SDL_Rect {h = 32, w = 32, x = 1820, y = 50}, false));
        _buttons.Add(new Buttons("pause", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
            new SDL_Rect {h = 32, w = 32, x = 1770, y = 50}, false));
        var cnt = 1;
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 2; j++)
            {
                _buttons.Add(new Buttons($"arab_{cnt}", new SDL_Rect {h = 210, w = 210, x = 0, y = 0},
                    new SDL_Rect {h = 90, w = 90, x = 1305 + i * 90, y = 825 + j * 90}, true));
                cnt++;
            }
        }
    }

//230 height 460-190
    public static HudManager GetInstance(ref List<Buttons> buttons, ref Hud hud)
    {
        if (_hudManager != null) return _hudManager;
        return _hudManager = new HudManager(ref buttons, ref hud);
    }

    public void RunManager()
    {
        Update();
    }

    public void PressButton(Buttons button, ref bool gameState, ref Building curent)
    {
        button.IsPressed = true;
        DoAction(button, ref gameState, ref curent);
    }

    private void DoAction(Buttons button, ref bool gameState, ref Building curent)
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
                        break;
                    case "pause":
                        gameState = !gameState;
                        button.TextureName = "resume";
                        break;
                    case "resume":
                        gameState = !gameState;
                        button.TextureName = "pause";
                        break;
                    // case "arab_1":
                    //     break;
                    // case "arab_2":
                    //     break;
                    // case "arab_3":
                    //     break;
                    // case "arab_4":
                    //     break;
                    // case "arab_5":
                    //     break;
                    // case "arab_6":
                    //     break;
                    // case "arab_7":
                    //     break;
                    // case "arab_8":
                    //     break;
                    // case "arab_9":
                    //     break;
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
        foreach (var button in _buttons)
        {
            CheckCollision(button);
        }
    }

    private void CheckCollision(Buttons button)
    {
        SDL_GetMouseState(out _mouseX, out _mouseY);
        if (_mouseX > button.DRect.x && _mouseX < button.DRect.x + button.DRect.w && _mouseY > button.DRect.y &&
            _mouseY < button.DRect.y + button.DRect.h)
        {
            if (button.IsFocused && button.IsPressed)
            {
                button.UpdateTextureName(Buttons.ButtonTextures.Pressed);
            }
            else
            {
                button.UpdateTextureName(Buttons.ButtonTextures.Focused);
            }
        }
        else
        {
            button.UpdateTextureName(Buttons.ButtonTextures.Unfocused);
        }
    }
}