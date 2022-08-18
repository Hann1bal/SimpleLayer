using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;

namespace SimpleLayer.GameEngine.Objects.Attributes;

public class BuildingAttributes
{
    public int BuildingCost = 10;
    public uint SpawnRate { get; init; }
    public int LastTick { get; set; }
    public BuildingType BuildingType { get; init; }
    public BuildingPlaceState BuildingPlaceState { get; set; }

    public int GoldPerMinute { get; set; }
}