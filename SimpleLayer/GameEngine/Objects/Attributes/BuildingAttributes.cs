using SimpleLayer.GameEngine.Objects.Types;

namespace SimpleLayer.GameEngine.Objects.Attributes;

public class BuildingAttributes
{
    public uint SpawnRate { get; init; }
    public int BuildingCost = 10;
    public int LastTick { get; set; }
    public BuildingType BuildingType { get; init; }
    public int GoldPerMinute { get; set; }
}