using SDL2;

namespace SimpleLayer.Objects;

public class Building : GameBaseObject
{
    public readonly bool IsFactory;
    public readonly uint SpawnRate = 5000;
    public int _xPos;
    public int _yPos;
    public List<Unit> Units = new();

    public Building(string textureName, int xPos, int yPos,
        int healtPpoint, int team, bool isFactory = true) :
        base(textureName, xPos, yPos, healtPpoint, team, true)
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
        Unit unit;
        switch (TextureName)
        {
            case "arab_1":
                unit = new Unit("adventurer", _xPos, _yPos, 5, Team, 5, 8, 9);
                break;
            case "arab_2":
                unit = new Unit("dwarf", _xPos, _yPos, 5, Team, 5, 8, 7);
                break;
            default:
                unit = new Unit("dwarf", _xPos, _yPos, 5, Team, 5, 8, 7);
                break;
        }

        Units.Add(unit);
        return unit;
    }
}