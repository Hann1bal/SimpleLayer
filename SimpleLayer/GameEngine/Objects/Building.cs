using SDL2;
using SimpleLayer.Objects.States;

namespace SimpleLayer.Objects;

public class Building : GameBaseObject
{
    public BuildingAttributes BuildingAttributes;

    public Building(string textureName, int xPos, int yPos,
        int healthPpoint, int team, int timer, BuildingType buildingType) :
        base(textureName, xPos, yPos, healthPpoint, team, ObjectType.Building)
    {
        BuildingAttributes = new BuildingAttributes {SpawnRate = 5, LastTick = timer, BuildingType = buildingType};
    }

    public Unit Spawn()
    {
        var unit = BaseObjectAttribute.TextureName switch
        {
            "arab_1" => new Unit("adventurer", (int) Math.Round(BaseObjectAttribute.XPosition),
                (int) Math.Round(BaseObjectAttribute.YPosition), 25, BaseObjectAttribute.Team, 5, 8,
                7),
            "arab_2" => new Unit("dwarf", (int) Math.Round(BaseObjectAttribute.XPosition),
                (int) Math.Round(BaseObjectAttribute.YPosition), 25, BaseObjectAttribute.Team, 5, 8, 7),
            _ => new Unit("dwarf", (int) Math.Round(BaseObjectAttribute.XPosition),
                (int) Math.Round(BaseObjectAttribute.YPosition), 5, BaseObjectAttribute.Team, 5, 8, 7)
        };
        return unit;
    }
}