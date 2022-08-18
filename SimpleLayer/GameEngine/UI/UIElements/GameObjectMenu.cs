using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.UI.UIAttributes;

namespace SimpleLayer.GameEngine.UI.UIElements;

public class GameObjectMenu
{
    private static GameObjectMenu _gbMenu;
    private List<Buttons> _menuButton;

    public GameObjectMenuAttribute GameObjectMenuAttribute =
        new() {CurrentObject = null, MaxX = 1285, MaxY = 1080, MinX = 400, MinY = 815};

    protected GameObjectMenu(List<Buttons> buttons)
    {
        _menuButton = buttons;
    }

    public static GameObjectMenu GetInstance(List<Buttons> buttons)
    {
        return _gbMenu ??= new GameObjectMenu(buttons);
    }

    public void SetCurrentObjects(GameBaseObject gameBaseObject)
    {
        GameObjectMenuAttribute.CurrentObject = gameBaseObject;
    }

    public void UnsetCurrentObjects()
    {
        GameObjectMenuAttribute.CurrentObject = null;
    }

    public void BuildMenu()
    {
    }

    public void DestroyMenu()
    {
    }
}