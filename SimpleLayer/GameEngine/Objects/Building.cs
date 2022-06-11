using System.Collections.ObjectModel;
using System.Numerics;
using SDL2;
using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class Building : GameBaseObject
{
    public List<Unit> Units = new();
    public int _xPos;
    public int _yPos;
    public readonly uint SpawnRate = 5000;
    public uint LastTick { get; set; }
    public int _team;
    public readonly bool IsFactory;

    public Building(string textureName, int xPos, int yPos,
        int healtPpoint, int team, bool isFactory = true) :
        base(textureName, xPos, yPos, healtPpoint, team, true, 0)
    {
        _team = team;
        _xPos = xPos;
        _yPos = yPos;
        XPosition = _xPos;
        YPosition = _yPos;
        LastTick = SDL.SDL_GetTicks();
        IsFactory = isFactory;
    }

    public Unit Spawn()
    {
        Unit unit;
        switch (TextureName)
        {
            case "arab_1":
                unit = new Unit("adventurer", _xPos, _yPos, 5, _team, 5, 8, 9);
                break;
            case "arab_2":
                unit = new Unit("dwarf", _xPos, _yPos, 5, _team, 5, 8, 7);
                break;
            default:
                unit = new Unit("dwarf", _xPos, _yPos, 5, _team, 5, 8, 7);
                break;
        }

        Units.Add(unit);
        return unit;
    }
}