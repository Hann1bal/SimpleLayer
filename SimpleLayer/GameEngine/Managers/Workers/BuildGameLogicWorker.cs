using System.Numerics;
using System.Security.Cryptography;
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
    private Stack<Event> _events;
    private Stack<Event> _receiveEvents;
    private Level _level;
    public BuildGameLogicWorker(ref List<Building> buildings, ref Dictionary<Vector2, List<GameBaseObject>> quadrant,
        ref List<Unit> units, ref Stack<Event> events, ref Stack<Event> receiveEvents,  ref Level level)
    {
        _buildings = buildings;
        _quadrant = quadrant;
        _units = units;
        _events = events;
        _receiveEvents = receiveEvents;
        _level = level;
    }

    public void DoJob()
    {
        HandleEvent();
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

    public void PlaceBuilding(int x, int y, ref Building? currenBuilding)
    {
        Building building;
        var team = x switch
        {
            < 800 => 1,
            > 2400 => 2,
            _ => 0
        };
        if (team == 0 || _level._tileLevel[new Vector2(x / 32, y / 32)].ContainBuilding ||
            !_level._tileLevel[new Vector2(x / 32, y / 32)].isPlacibleTile) return;
        building = new Building(currenBuilding.TextureName, _level._tileLevel[new Vector2(x / 32, y / 32)]._sdlDRect.x,
            _level._tileLevel[new Vector2(x / 32, y / 32)]._sdlDRect.y, currenBuilding.HealthPoint, team);
        _buildings.Add(building);
        AddToQuadrant(building);
        _level._tileLevel[new Vector2(x / 32, y / 32)].ContainBuilding = true;
        _events.Push(new Event(){Id = 1+x, TargetName = currenBuilding.TextureName, TargetType = "building", X = x, Y = y});
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

    private void HandleEvent()
    {
        if (_receiveEvents == null) return;

        while (_receiveEvents.Count > 0)
        {

            var oppEvent = _receiveEvents.Pop();
            var team = oppEvent.X switch
            {
                < 800 => 1,
                > 2400 => 2,
                _ => 0
            };
            var building = new Building(oppEvent.TargetName, oppEvent.X, oppEvent.Y, 5000, team, true);
            PlaceBuilding(oppEvent.X, oppEvent.Y, ref building );

        }
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