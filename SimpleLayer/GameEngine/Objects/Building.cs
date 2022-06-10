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
        base(textureName, xPos, yPos, healtPpoint, team)
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
        var unit = new Unit("dude", _xPos, _yPos, 5, _team, 5);
        Units.Add(unit);
        return unit;
    }
    
}