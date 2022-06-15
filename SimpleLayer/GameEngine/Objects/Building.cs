using SDL2;

namespace SimpleLayer.Objects;

public class Building : GameBaseObject
{
    public readonly bool IsFactory;
    public readonly uint SpawnRate = 5000;
    public int _xPos;
    public int _yPos;

    public Building(string textureName, int xPos, int yPos,
        int healthPpoint, int team, bool isFactory = true) :
        base(textureName, xPos, yPos, healthPpoint, team, true)
    {
        _xPos = xPos;
        _yPos = yPos;
        XPosition = _xPos;
        YPosition = _yPos;
        LastTick = SDL.SDL_GetTicks();
        IsFactory = isFactory;
    }

    public uint LastTick { get; set; }

    public Unit Spawn()
    {
        var unit = TextureName switch
        {
            "arab_1" => new Unit("adventurer", _xPos, _yPos, 5, Team, 5, 8, 9),
            "arab_2" => new Unit("dwarf", _xPos, _yPos, 5, Team, 5, 8, 7),
            _ => new Unit("dwarf", _xPos, _yPos, 5, Team, 5, 8, 7)
        };
        return unit;
    }
}