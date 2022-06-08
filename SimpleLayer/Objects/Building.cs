using System.Collections.ObjectModel;
using System.Numerics;
using SDL2;
using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class Building : GameBaseObject
{
    public List<Unit> Units = new();
    private IntPtr _renderer;
    private readonly int _xPos;
    private readonly int _yPos;
    private int _healtPoint;
    public readonly uint SpawnRate = 5000;
    public uint LastTick { get; set; }
    private new readonly int _team;
    public readonly bool IsFactory;

    public Building(ref IntPtr renderer, string textureName, int xPos, int yPos,
        int healtPpoint, int team, bool isFactory = true) :
        base(ref renderer, textureName, xPos, yPos, healtPpoint, team)
    {
        _team = team;
        _renderer = renderer;
        _xPos = xPos;
        _yPos = yPos;
        _healtPoint = healtPpoint;
        LastTick = SDL.SDL_GetTicks();
        IsFactory = isFactory;
    }

    public Unit Spawn()
    {
        var unit = new Unit(ref _renderer, "dude", _xPos, _yPos, 5, _team);
        Units.Add(unit);
        return unit;
    }
    
    public void RenderAllUnits(ref Camera camera, ref Texture textureManager)
    {
        foreach (var unit in Units.ToArray())
        {
            unit.Render(ref camera, ref textureManager);
        }
    }
    
}