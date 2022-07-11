using SimpleLayer.Objects.States;

namespace SimpleLayer.Objects;

public struct BuildingAttributes
{
    public uint SpawnRate { get; init; }
    public int LastTick { get; set; }
    public BuildingType BuildingType { get; init; }
    public int GoldPerMinute { get; set; }
}