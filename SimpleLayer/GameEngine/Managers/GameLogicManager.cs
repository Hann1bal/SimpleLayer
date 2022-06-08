using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using SDL2;
using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine;

public class GameLogicManager
{
    private List<Building> _buildings;
    private Dictionary<Vector2, List<GameBaseObject>> _quadrant = new();
    private static GameLogicManager _gameLogicManager;
    private Building _buildingBase;
    private Building _buildingBase2;
    private IntPtr _renderer;

    private GameLogicManager(ref List<Building> buildings, IntPtr renderer)
    {
        _buildings = buildings;
        _renderer = renderer;
        InitQuadrant();
        InitPlayerBase();
    }

    public static GameLogicManager GetInstance(ref List<Building> buildings, IntPtr _renderer)
    {
        if (_gameLogicManager != null) return _gameLogicManager;
        return _gameLogicManager = new GameLogicManager(ref buildings, _renderer);
    }

    private void InitQuadrant()
    {
        for (var i = -1; i <= 10; i++)
        {
            for (var j = -1; j <= 10; j++)
            {
                _quadrant.Add(new Vector2(i, j), new List<GameBaseObject>());
            }
        }
    }

    private void InitPlayerBase()
    {
        _buildingBase = new Building(ref _renderer, "tron",
            400, 1600, 50, 1, false);
        _buildingBase2 = new Building(ref _renderer, "tron",
            2800, 1600, 50, 2, false);
        AddToQuadrant(_buildingBase);
        AddToQuadrant(_buildingBase2);
        _buildings.Add(_buildingBase);
        _buildings.Add(_buildingBase2);
    }

    public void RunManager()
    {
        SpawnUnits();
        Update();
    }

    private void SpawnUnits()
    {
        var tick = SDL.SDL_GetTicks();
        foreach (var building in _buildings.Where(building => building.IsFactory)
                     .Where(building => tick - building.LastTick >= building.SpawnRate).ToArray())
        {
            AddToQuadrant(building.Spawn());
            building.LastTick = tick;
        }
    }

    private void MoveAllunits()
    {
        foreach (var building in _buildings.ToArray())
        {
            foreach (var unit in building.Units.ToArray())
            {
                switch (unit._dRect.x + unit._dRect.w)
                {
                    case > 3199:
                        unit._target = null;
                        building.Units.Remove(unit);
                        _quadrant[unit._lastQuadrant].Remove(unit);
                        unit.Dispose();
                        break;
                    case < 0:
                        unit._target = null;
                        building.Units.Remove(unit);
                        _quadrant[unit._lastQuadrant].Remove(unit);
                        unit.Dispose();
                        break;
                    default:
                        break;
                }

                CheckCollision(unit);
                
                if (unit.isDead)
                {
                    unit._target = null;
                    building.Units.Remove(unit);
                    _quadrant[unit._lastQuadrant].Remove(unit);
                    unit.Dispose();
                }

                NearestCoords(unit);
                MoveUnit(unit);
                CheckForSwapQuadrant(unit);
            }
        }
    }

    public void MoveUnit(Unit unit)
    {
        // Console.WriteLine($"self - {xPosition},{yPosition}, target - {_target}, _targetDistance- {_targetDistance}");
        if (unit._targetDistance == 0) return;
        var x = (int) Math.Round((float) (unit._target.xPosition - unit.xPosition) / unit._targetDistance);
        var y = (int) Math.Round((float) (unit._target.yPosition - unit.yPosition) / unit._targetDistance);
        unit.xPosition += x;
        unit.yPosition += y;
        unit._dRect.x = unit.xPosition;
        unit._dRect.y = unit.yPosition;
    }


    private void NearestCoords(Unit unit)
    {
        int minimumDistance = Int32.MaxValue;
        GameBaseObject nearestTarget = null;
        for (var i = unit._lastQuadrant.X - 1; i <= unit._lastQuadrant.X + 1; i++)
        {
            for (var j = unit._lastQuadrant.Y - 1; j <= unit._lastQuadrant.Y + 1; j++)
            {
                foreach (var enemy in _quadrant[new Vector2(i, j)].Where(b => b._team != unit._team).ToArray())
                {
                    var distance = DistanceBetween(unit, enemy);
                    if (nearestTarget == null || minimumDistance > distance)
                    {
                        nearestTarget = enemy;
                        minimumDistance = distance;
                    }
                }
            }
        }

        if (nearestTarget == null)
        {
            nearestTarget = unit._team == 1 ? _buildingBase2 : _buildingBase;
            minimumDistance = DistanceBetween(unit, nearestTarget);
        }

        unit._targetDistance = minimumDistance;
        unit._target = nearestTarget;
    }

    private static int DistanceBetween(GameBaseObject unit, GameBaseObject target)
    {
        return (int) Math.Round(Vector2.Distance(new Vector2(unit.xPosition, unit.yPosition),
            new Vector2(target.xPosition, target.yPosition)));
    }


    private void Update()
    {
        SpawnUnits();
        MoveAllunits();
    }


    public void CheckForSwapQuadrant(GameBaseObject gameBaseObject)
    {
        var qudX = gameBaseObject.xPosition / 320;
        var qudY = gameBaseObject.yPosition / 320;
        Console.WriteLine($"Old quadrant{gameBaseObject._lastQuadrant}, new {new Vector2(qudX, qudY)}");
        if (!(Vector2.Distance(gameBaseObject._lastQuadrant, new Vector2(qudX, qudY)) > 0)) return;
        DeleteFromQuadrant(gameBaseObject);
        AddToQuadrant(gameBaseObject);
    }

    public void AddToQuadrant(GameBaseObject gameBaseObject)
    {
        var qudX = gameBaseObject.xPosition / 320;
        var qudY = gameBaseObject.yPosition / 320;
        _quadrant[new Vector2(qudX, qudY)].Add(gameBaseObject);
        gameBaseObject._lastQuadrant = new Vector2(qudX, qudY);
    }

    private void DeleteFromQuadrant(GameBaseObject gameBaseObject)
    {
        _quadrant[gameBaseObject._lastQuadrant].Remove(gameBaseObject);
    }

    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: WhereListIterator`1[SimpleLayer.Objects.GameBaseObject]")]
    private void CheckCollision(Unit unit)
    {
        for (var i = unit._lastQuadrant.X - 1; i <= unit._lastQuadrant.X + 1; i++)
        {
            for (var j = unit._lastQuadrant.Y - 1; j <= unit._lastQuadrant.Y + 1; j++)
            {
                foreach (var enemy in _quadrant[new Vector2(i, j)].Where(b => b._team != unit._team))
                 {
                    switch (SDL.SDL_HasIntersection(ref unit._dRect, ref enemy._dRect))
                    {
                        case SDL.SDL_bool.SDL_TRUE:
                            unit.isDead = true;
                            enemy.isDead = true;
                            break;
                        case SDL.SDL_bool.SDL_FALSE:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}