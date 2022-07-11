using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.Objects.States;

namespace SimpleLayer.Objects;

public class ButtonAttribute
{
    public ButtonState ButtonState { get; set; }
    public ButtonPressState ButtonPressState { get; set; }
    public ButtonType ButtonType { get; init; }
}