using System.Numerics;
using SDL2;
using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class BuildGameLogicWorker
{
    private readonly List<Building> _buildings;
    private readonly Dictionary<Vector2, List<GameBaseObject>> _quadrant = new();
    private readonly List<Unit> _units;
    public Building BuildingBase;
    public Building BuildingBase2;

    public BuildGameLogicWorker(ref List<Building> buildings, ref Dictionary<Vector2, List<GameBaseObject>> quadrant,
        ref List<Unit> units)
    {
        _buildings = buildings;
        _quadrant = quadrant;
        _units = units;
    }

    public void DoJob()
    {
        DestroyDeadBuildings();
        SpawnUnits();
    }

    public void InitPlayerBase()
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

    public void PlaceBuilding(int x, int y, ref Building? currenBuilding, ref Level level)
    {
        Building building;
        var team = x switch
        {
            < 800 => 1,
            > 2400 => 2,
            _ => 0
        };
        if (team == 0 || level._tileLevel[new Vector2(x / 32, y / 32)].ContainBuilding ||
            !level._tileLevel[new Vector2(x / 32, y / 32)].isPlacibleTile) return;
        building = new Building(currenBuilding.TextureName, level._tileLevel[new Vector2(x / 32, y / 32)]._sdlDRect.x,
            level._tileLevel[new Vector2(x / 32, y / 32)]._sdlDRect.y, currenBuilding.HealthPoint, team);
        _buildings.Add(building);
        AddToQuadrant(building);
        level._tileLevel[new Vector2(x / 32, y / 32)].ContainBuilding = true;
    }

    private void SpawnUnits()
    {
        var tick = SDL.SDL_GetTicks();
        foreach (var building in _buildings.Where(building => building.IsFactory)
                     .Where(building => tick - building.LastTick >= building.SpawnRate)
                     .Where(building => !building.IsDead).ToArray())
        {
            var unit = building.Spawn();
            _units.Add(unit);
            AddToQuadrant(unit);
            building.LastTick = tick;
        }
    }

    private void AddToQuadrant(GameBaseObject gameBaseObject)
    {
        var qudX = gameBaseObject.XPosition / 320;
        var qudY = gameBaseObject.YPosition / 320;
        _quadrant[new Vector2(qudX, qudY)].Add(gameBaseObject);
        gameBaseObject.LastQuadrant = new Vector2(qudX, qudY);
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
}