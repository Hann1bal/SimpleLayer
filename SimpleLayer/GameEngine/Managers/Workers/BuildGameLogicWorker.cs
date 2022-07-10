using System.Numerics;
using System.Security.Cryptography;
using SDL2;
using SimpleLayer.Objects;
using SimpleLayer.Objects.States;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class BuildGameLogicWorker
{
    private readonly List<Building> _buildings;
    private readonly Dictionary<Vector2, List<GameBaseObject>> _quadrant = new();
    private readonly List<Unit> _units;
    public Building BuildingBase;
    public Building BuildingBase2;
    private Broker Broker;
    private Stack<Event> _events;
    private Stack<Event> _receiveEvents;
    private Level _level;

    public BuildGameLogicWorker(ref List<Building> buildings, ref Dictionary<Vector2, List<GameBaseObject>> quadrant,
        ref List<Unit> units, ref Stack<Event> events, ref Stack<Event> receiveEvents, ref Level level)
    {
        _buildings = buildings;
        _quadrant = quadrant;
        _units = units;
        _events = events;
        _receiveEvents = receiveEvents;
        _level = level;
        Broker = new Broker(_events, _receiveEvents);
    }

    public void RunJob(Time timer)
    {
        // HandleEvent();
        DestroyDeadBuildings();
        SpawnUnits(timer);
    }

    public void InitPlayerBase()
    {
        BuildingBase = new Building("tron",
            400, 1600, 50000, 1, 0, BuildingType.Base);
        BuildingBase2 = new Building("tron",
            2800, 1600, 50000, 2, 0,BuildingType.Base);
        AddToQuadrant(BuildingBase);
        AddToQuadrant(BuildingBase2);
        _buildings.Add(BuildingBase);
        _buildings.Add(BuildingBase2);
    }

    public void PlaceBuilding(int x, int y, ref Building? currenBuilding, Time timer)
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
        building = new Building(currenBuilding.BaseObjectAttribute.TextureName, _level._tileLevel[new Vector2(x / 32, y / 32)]._sdlDRect.x,
            _level._tileLevel[new Vector2(x / 32, y / 32)]._sdlDRect.y, currenBuilding.BaseObjectAttribute.HealthPoint, team,timer.Seconds,BuildingType.Factory);
        _buildings.Add(building);
        AddToQuadrant(building);
        _level._tileLevel[new Vector2(x / 32, y / 32)].ContainBuilding = true;
        Broker.AddNewEvent(new Event
            {Id = 1 + x, TargetName = currenBuilding.BaseObjectAttribute.TextureName, TargetType = "building", X = x, Y = y});
    }

    private void SpawnUnits(Time timer)
    {
        foreach (var building in _buildings.Where(building => building.BuildingAttributes.BuildingType == BuildingType.Factory)
                     .Where(building => timer.Seconds - building.BuildingAttributes.LastTick >= building.BuildingAttributes.SpawnRate)
                     .Where(building => building.BaseObjectAttribute.DoAState == DoAState.Alive).ToArray())
        {
            // if (_units.Count >= 2) return;
            var unit = building.Spawn();
            _units.Add(unit);
            AddToQuadrant(unit);
            building.BuildingAttributes.LastTick = timer.Seconds;
        }
    }

    private void AddToQuadrant(GameBaseObject gameBaseObject)
    {
        var qudX = (int)Math.Round(gameBaseObject.BaseObjectAttribute.XPosition / 320);
        var qudY = (int)Math.Round(gameBaseObject.BaseObjectAttribute.YPosition / 320);
        _quadrant[new Vector2(qudX, qudY)].Add(gameBaseObject);
        gameBaseObject.BaseObjectAttribute.LastQuadrant = new Vector2(qudX, qudY);
    }

    // private void HandleEvent()
    // {
    //     if (Broker.ReceiveEvents == null) return;
    //
    //     while (Broker.ReceiveEvents.Count > 0)
    //     {
    //         var oppEvent = Broker.GetEvent();
    //         var team = oppEvent.X switch
    //         {
    //             < 800 => 1,
    //             > 2400 => 2,
    //             _ => 0
    //         };
    //         var building = new Building(oppEvent.TargetName, oppEvent.X, oppEvent.Y, 5000, team, false);
    //         PlaceBuilding(oppEvent.X, oppEvent.Y, ref building);
    //     }
    // }

    private void DestroyDeadBuildings()
    {
        foreach (var building in _buildings.Where(building => building.BaseObjectAttribute.DoAState == DoAState.Dead).ToArray())
        {
            _buildings.Remove(building);
            _quadrant[building.BaseObjectAttribute.LastQuadrant].Remove(building);

            building.Dispose();
        }
    }
}