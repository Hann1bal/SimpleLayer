using SimpleLayer.GameEngine.Objects.Attributes;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.Templates.ObjectsTemplate;

namespace SimpleLayer.GameEngine.Objects;

public class Building : GameBaseObject
{
    public BuildingAttributes BuildingAttributes;
    public AdventurerTemplate AdventurerTemplate= new ();
    public DwarfTemplate DwarfTemplate = new ();

    public Building(string textureName, int xPos, int yPos,
        int healthPpoint, int team, int timer, BuildingType buildingType, int hightSprite, int widthSprite) :
        base(textureName, xPos, yPos, healthPpoint, team, ObjectType.Building, hightSprite, widthSprite)
    {
        BuildingAttributes = new BuildingAttributes {SpawnRate = 5, LastTick = timer, BuildingType = buildingType};
    }

    public Unit Spawn()
    {
        var unit = BaseObjectAttribute.TextureName switch
        {
            "arab_1" => new Unit(AdventurerTemplate.TextureName, (int) Math.Round(BaseObjectAttribute.XPosition),
                (int) Math.Round(BaseObjectAttribute.YPosition), AdventurerTemplate.HealthPoint,
                BaseObjectAttribute.Team, AdventurerTemplate.Damage, AdventurerTemplate.MaxMovingFrame,
                AdventurerTemplate.MaxAttackFrame, AdventurerTemplate.HeightSprite, AdventurerTemplate.WidthSprite),
            "arab_2" => new Unit(DwarfTemplate.TextureName, (int) Math.Round(BaseObjectAttribute.XPosition),
                (int) Math.Round(BaseObjectAttribute.YPosition), DwarfTemplate.HealthPoint, BaseObjectAttribute.Team,
                DwarfTemplate.Damage, DwarfTemplate.MaxMovingFrame,
                DwarfTemplate.MaxAttackFrame, DwarfTemplate.HeightSprite, DwarfTemplate.WidthSprite),
            _ => new Unit(DwarfTemplate.TextureName, (int) Math.Round(BaseObjectAttribute.XPosition),
                (int) Math.Round(BaseObjectAttribute.YPosition), DwarfTemplate.HealthPoint, BaseObjectAttribute.Team,
                DwarfTemplate.Damage, DwarfTemplate.MaxMovingFrame,
                DwarfTemplate.MaxAttackFrame, DwarfTemplate.HeightSprite, DwarfTemplate.WidthSprite)
        };
        return unit;
    }
}