using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class HudManager
{
    private static HudManager _hudManager = null;
    private List<Buttons> _buttons;
    private int _mouseX;
    private int _mouseY;
    private HudManager(ref List<Buttons> buttons, ref Hud hud)
    {
        _buttons = buttons;
        Init();
        
    }

    private void Init()
    {
        _buttons.Add(new Buttons("settings", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
            new SDL_Rect {h = 32, w = 32, x = 1820, y = 50}));
        _buttons.Add(new Buttons("pause", new SDL_Rect {h = 32, w = 32, x = 0, y = 0},
            new SDL_Rect {h = 32, w = 32, x = 1770, y = 50}));
    }

    public static HudManager GetInstance(ref List<Buttons> buttons, ref Hud hud)
    {
        if (_hudManager != null) return _hudManager;
        return _hudManager = new HudManager(ref buttons, ref hud);
    }

    public void RunManager()
    {
        Update();
    }

    public void PressButton(Buttons button, ref bool gameState)
    {
        button.IsPressed = true;
        DoAction(button, ref gameState);
    }

    private void DoAction(Buttons button, ref bool gameState)
    {
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