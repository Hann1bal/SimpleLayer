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
    public Building BuildingBase;
    public Building BuildingBase2;
    private bool _state = false;
    
    private GameLogicManager(ref List<Building> buildings)
    {
        _buildings = buildings;
        InitQuadrant();
        InitPlayerBase();
    }

    public static GameLogicManager GetInstance(ref List<Building> buildings)
    {
        if (_gameLogicManager != null) return _gameLogicManager;
        return _gameLogicManager = new GameLogicManager(ref buildings);
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
        BuildingBase = new Building("tron",
            400, 1600, 50000, 1, false);
        BuildingBase2 = new Building("tron",
            2800, 1600, 50000, 2, false);
        AddToQuadrant(BuildingBase);
        AddToQuadrant(BuildingBase2);
        _buildings.Add(BuildingBase);
        _buildings.Add(BuildingBase2);
    }

    public void RunManager()
    {
        DestroyDeadBuildings();
        SpawnUnits();
        Update();
    }

    private void SpawnUnits()
    {
        var tick = SDL.SDL_GetTicks();
        foreach (var building in _buildings.Where(building => building.IsFactory)
                     .Where(building => tick - building.LastTick >= building.SpawnRate)
                     .Where(building => !building.IsDead).ToArray())
        {
            AddToQuadrant(building.Spawn());
            building.LastTick = tick;
        }
    }

    public void PlaceBuilding(int x, int y, ref Building currenBuilding)
    {
        Building building;
        var team = x switch
        {
            < 800 => 1,
            > 2400 => 2,
            _ => 0
        };
        building = new Building(currenBuilding.TextureName, x, y, currenBuilding.HealthPoint, team, true);
        _buildings.Add(building);
        AddToQuadrant(building);
    }


    private void DestroyDeadBuildings()
    {
        foreach (var building in _buildings.Where(building => building.IsDead).ToArray())
        {
            _buildings.Remove(building);
            _quadrant[building.LastQuadrant].Remove(building);
            building.Dispose();
        }
    }

    private void MoveAllunits()
    {
        foreach (var building in _buildings.ToArray())
        {
            foreach (var unit in building.Units.ToArray())
            {
                switch (unit.DRect.x + unit.DRect.w)
                {
                    case > 3199:
                    case < 0:
                        unit.Target = null;
                        building.Units.Remove(unit);
                        _quadrant[unit.LastQuadrant].Remove(unit);
                        unit.Dispose();
                        break;
                    default:
                        break;
                }

                CheckCollision(unit);
                if (unit.IsDead)
                {
                    unit.Target = null;
                    building.Units.Remove(unit);
                    _quadrant[unit.LastQuadrant].Remove(unit);
                    unit.Dispose();
                }

                NearestCoords(unit);
                MoveUnit(unit);
                CheckForSwapQuadrant(unit);
            }
        }
    }

    private void MoveUnit(Unit unit)
    {
        if (unit.TargetDistance == 0) return;
        unit.CurrentXSpeed = (float) (unit.Target.XPosition - unit.XPosition) / unit.TargetDistance;
        unit.CurrentYSpeed = (float) (unit.Target.YPosition - unit.YPosition) / unit.TargetDistance;
        unit.XPosition += 2 * (int) Math.Round(unit.CurrentXSpeed);
        unit.YPosition += 2 * (int) Math.Round(unit.CurrentYSpeed);
        unit.DRect.x = unit.XPosition;
        unit.DRect.y = unit.YPosition;
        if (unit.CurrentFrame < unit.MaxFrame) unit.CurrentFrame++;
        else unit.CurrentFrame = 1;
    }


    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        MessageId = "type: SimpleLayer.Objects.GameBaseObject[]")]
    private void NearestCoords(Unit unit)
    {
        var minimumDistance = Int32.MaxValue;
        GameBaseObject nearestTarget = null;
        for (var i = unit.LastQuadrant.X - 1; i <= unit.LastQuadrant.X + 1; i++)
        {
            for (var j = unit.LastQuadrant.Y - 1; j <= unit.LastQuadrant.Y + 1; j++)
            {
                foreach (var enemy in _quadrant[new Vector2(i, j)].Where(b => b.Team != unit.Team)
                             .Where(cb => cb.IsDead == false).ToArray())
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
            nearestTarget = unit.Team == 1 ? BuildingBase2 : BuildingBase;
            minimumDistance = DistanceBetween(unit, nearestTarget);
        }

        unit.TargetDistance = minimumDistance;
        unit.Target = nearestTarget;
    }

    private static int DistanceBetween(GameBaseObject unit, GameBaseObject target)
    {
        return (int) Math.Round(Vector2.Distance(new Vector2(unit.XPosition, unit.YPosition),
            new Vector2(target.XPosition, target.YPosition)));
    }


    private void Update()
    {
        SpawnUnits();
        MoveAllunits();
    }


    private void CheckForSwapQuadrant(GameBaseObject gameBaseObject)
    {
        if (!(Vector2.Distance(gameBaseObject.LastQuadrant,
                new(gameBaseObject.XPosition / 320, gameBaseObject.YPosition / 320)) > 0)) return;
        DeleteFromQuadrant(gameBaseObject);
        AddToQuadrant(gameBaseObject);
    }

    private void AddToQuadrant(GameBaseObject gameBaseObject)
    {
        var qudX = gameBaseObject.XPosition / 320;
        var qudY = gameBaseObject.YPosition / 320;
        _quadrant[new Vector2(qudX, qudY)].Add(gameBaseObject);
        gameBaseObject.LastQuadrant = new Vector2(qudX, qudY);
    }

    private void DeleteFromQuadrant(GameBaseObject gameBaseObject)
    {
        _quadrant[gameBaseObject.LastQuadrant].Remove(gameBaseObject);
    }

    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        MessageId = "type: WhereListIterator`1[SimpleLayer.Objects.GameBaseObject]")]
    private void CheckCollision(Unit unit)
    {
        for (var i = unit.LastQuadrant.X - 1; i <= unit.LastQuadrant.X + 1; i++)
        {
            for (var j = unit.LastQuadrant.Y - 1; j <= unit.LastQuadrant.Y + 1; j++)
            {
                foreach (var enemy in _quadrant[new Vector2(i, j)].Where(b => b.Team != unit.Team))
                {
                    switch (SDL.SDL_HasIntersection(ref unit.DRect, ref enemy.DRect))
                    {
                        case SDL.SDL_bool.SDL_TRUE:
                            unit.HealthPoint -= enemy.Damage;
                            enemy.HealthPoint -= unit.Damage;
                            if (unit.HealthPoint == 0)
                            {
                                unit.IsDead = true;
                            }

                            if (enemy.HealthPoint == 0)
                            {
                                enemy.IsDead = true;
                            }

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

    public void GetState(ref Game.GameState state)
    {
        if (BuildingBase == null || BuildingBase2 == null || BuildingBase.HealthPoint <= 0 ||
            BuildingBase2.HealthPoint <= 0)
        {
            state = Game.GameState.GameOver;
            return;
        }
        state = Game.GameState.Play;
    }
}