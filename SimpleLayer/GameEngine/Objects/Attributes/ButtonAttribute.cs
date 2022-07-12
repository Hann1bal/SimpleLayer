using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;

namespace SimpleLayer.GameEngine.Objects.Attributes;

public class ButtonAttribute
{
    public ButtonState ButtonState { get; set; }
    public ButtonPressState ButtonPressState { get; set; }
    public ButtonType ButtonType { get; init; }
    public GameObjectsButtonType? GameObjectsButtonType { get; init; }
    public EoDButtonState? EoDButtonState { get; set; }
}