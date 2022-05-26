using System.Collections.ObjectModel;
using SDL2;
using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class Building : GameBaseObject
{
    public  Collection<Unit> _units = new Collection<Unit>();
    private IntPtr _renderer;
    private Texture _textureManager;
    private int _xPos, _yPos;
    private int _healtPoint;
    private uint _lastTick = 0;

    public Building(ref IntPtr renderer,  ref Texture textureManager, string textureName, int xPos, int yPos,
        int healtPpoint) :
        base(ref renderer,  ref textureManager, textureName, xPos, yPos, healtPpoint)
    {
        _renderer = renderer;
        _textureManager = textureManager;
        _xPos = xPos;
        _yPos = yPos;
        _healtPoint = healtPpoint;
        _lastTick = SDL.SDL_GetTicks();
    }

    public void Spawn()
    {
        if (SDL.SDL_GetTicks() - _lastTick <= 5000) return;
        _units.Add(new Unit(ref _renderer, ref _textureManager, "dude", _xPos, _yPos, 5));
        _lastTick = SDL.SDL_GetTicks();
    }

    public void MoveAllUnits()
    {
        foreach (var unit in _units.ToList())
        {
            unit.Move();
            if (unit._dRect.x + unit._dRect.w <= 3200) continue;
            unit._Speed = 0;
            Console.WriteLine(_units.Count);
            unit.Dispose();
            _units.Remove(unit);
            Console.WriteLine(_units.Count);
        }
    }

    public void RenderAllUnits(ref Camera camera)
    {
        foreach (var unit in _units)
        {
            unit.Render(ref camera);
        }
    }

    public void UpdateAllUnits()
    {
        foreach (var unit in _units)
        {
            unit.Update();
        }
    }
}