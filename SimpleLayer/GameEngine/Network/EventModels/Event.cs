using SimpleLayer.GameEngine.Network.Types;

namespace SimpleLayer.GameEngine.Network.EventModels;

[Serializable]
public struct BuildingEvent
{
    public int Id { get; init; }
    public NetworkEventType Type { get; init; }
    public string TargetType { get; init; }
    public string TargetName { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
}