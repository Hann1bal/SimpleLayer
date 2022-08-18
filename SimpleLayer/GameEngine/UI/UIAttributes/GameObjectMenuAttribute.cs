using SimpleLayer.GameEngine.Objects;

namespace SimpleLayer.GameEngine.UI.UIAttributes;

public struct GameObjectMenuAttribute
{
    public GameBaseObject? CurrentObject { get; set; }
    public int MinX { get; init; }
    public int MaxX { get; init; }
    public int MinY { get; init; }
    public int MaxY { get; init; }
}