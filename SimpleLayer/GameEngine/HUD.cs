using SDL2;
using SimpleLayer.GameEngine.UtilComponents;

namespace SimpleLayer.GameEngine;

public class Hud
{
    private List<Button> _buttons;
    private static Hud _hud;
    protected Hud()
    {
        _buttons = InitButtons();
    }

    private List<Button> InitButtons()
    {
        throw new NotImplementedException();
    }

    public static Hud GetInstance()
    {
        return _hud ??= new Hud();
    }
}