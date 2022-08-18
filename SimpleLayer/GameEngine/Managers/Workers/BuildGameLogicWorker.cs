using System.Numerics;
using SimpleLayer.GameEngine.Network;
using SimpleLayer.GameEngine.Network.EventModels;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.UtilComponents;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class BuildGameLogicWorker
{
    private readonly List<Building> _buildings;
    private readonly Stack<BuildingEvent> _events;
    private readonly Level _level;
    private readonly Dictionary<Vector2, List<GameBaseObject>> _quadrant = new();
    private readonly Stack<BuildingEvent> _receiveEvents;
    private readonly List<Unit> _units;
    private Broker Broker;
    public Building BuildingBase;
    public Building BuildingBase2;

    public BuildGameLogicWorker(ref List<Building> buildings, ref Dictionary<Vector2, List<GameBaseObject>> quadrant,
        ref List<Unit> units, ref Stack<BuildingEvent> events, ref Stack<BuildingEvent> receiveEvents, ref Level level)
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
            400, 1600, 50000, 1, 0, BuildingType.Base, BuildingPlaceState.Placed, 210, 210);
        BuildingBase2 = new Building("tron",
            2800, 1600, 50000, 2, 0, BuildingType.Base, BuildingPlaceState.NonPlaced, 210, 210);
        AddToQuadrant(BuildingBase);
        AddToQuadrant(BuildingBase2);
        _buildings.Add(BuildingBase);
        _buildings.Add(BuildingBase2);
    }

    public Building? SelectBuilding(float x, float y)
    {
        var qudX = (int) Math.Round(x / 320);
        var qudY = (int) Math.Round(y / 320);
        if (_quadrant[new Vector2(qudX, qudY)]
                .Where(c => c.BaseObjectAttribute.ObjectType == ObjectType.Building).ToList() != null &&
            _quadrant[new Vector2(qudX, qudY)]
                .Where(c => c.BaseObjectAttribute.ObjectType == ObjectType.Building).ToList().Count > 0)
        {
            var building = _quadrant[new Vector2(qudX, qudY)]
                .Where(c => c.BaseObjectAttribute.ObjectType == ObjectType.Building).ToList();
            foreach (var bld in building)
                if (x > bld.BaseObjectAttribute.XPosition && x < bld.BaseObjectAttribute.XPosition + bld.SRect.w / 5 &&
                    y > bld.BaseObjectAttribute.YPosition && y < bld.BaseObjectAttribute.YPosition + bld.SRect.h / 5)
                {
                    Console.WriteLine(1);
                    return bld as Building;
                }
        }

        return null;
    }

    public bool BuildingFoccused(float x, float y)
    {
        Building blds = null;
        var qudX = (int) Math.Round(x / 320);
        var qudY = (int) Math.Round(y / 320);
        if (_quadrant[new Vector2(qudX, qudY)]
                .Where(c => c.BaseObjectAttribute.ObjectType == ObjectType.Building).ToList() != null &&
            _quadrant[new Vector2(qudX, qudY)]
                .Where(c => c.BaseObjectAttribute.ObjectType == ObjectType.Building).ToList().Count > 0)
        {
            var building = _quadrant[new Vector2(qudX, qudY)]
                .Where(c => c.BaseObjectAttribute.ObjectType == ObjectType.Building).ToList();
            foreach (var bld in building.Where(bld => x > bld.BaseObjectAttribute.XPosition && x < bld.BaseObjectAttribute.XPosition + bld.SRect.w / 5 &&
                                                      y > bld.BaseObjectAttribute.YPosition && y < bld.BaseObjectAttribute.YPosition + bld.SRect.h / 5))
            {
                blds = bld as Building;
                return blds != null;
            }
        }

        return false;
    }

    public void UnSelectBuilding(ref Building? currenBuilding)
    {
        currenBuilding.BuildingAttributes.BuildingPlaceState = BuildingPlaceState.Placed;
        currenBuilding = null;
    }


    public void PlaceBuilding(int x, int y, ref Building? currenBuilding, Time timer, ref Player player)
    {
        Building building;
        var team = x switch
        {
            < 800 => 1,
            > 2400 => 2,
            _ => 0
        };
        if (team == 0 || _level._tileLevel[new Vector2(x / 32, y / 32)].TileAttribute.ContainBuilding ||
            !_level._tileLevel[new Vector2(x / 32, y / 32)].TileAttribute.isPlacibleTile) return;
        player.PlayerAttribute.Gold -= currenBuilding.BuildingAttributes.BuildingCost;
        building = new Building(currenBuilding.BaseObjectAttribute.TextureName,
            _level._tileLevel[new Vector2(x / 32, y / 32)].SdlDRect.x,
            _level._tileLevel[new Vector2(x / 32, y / 32)].SdlDRect.y, currenBuilding.BaseObjectAttribute.HealthPoint,
            team, timer.Seconds, BuildingType.Factory, BuildingPlaceState.Placed, 210, 210);
        _buildings.Add(building);
        AddToQuadrant(building);
        _level._tileLevel[new Vector2(x / 32, y / 32)].TileAttribute.ContainBuilding = true;
        // Broker.AddNewEvent(new Event
        // {Id = 1 + x, TargetName = currenBuilding.BaseObjectAttribute.TextureName, TargetType = "building", X = x, Y = y});
    }

    private void SpawnUnits(Time timer)
    {
        foreach (var building in _buildings
                     .Where(building => building.BuildingAttributes.BuildingType == BuildingType.Factory)
                     .Where(building => timer.Seconds - building.BuildingAttributes.LastTick >=
                                        building.BuildingAttributes.SpawnRate)
                     .Where(building => building.BaseObjectAttribute.DoAState == DoAState.Alive).ToArray())
        {
            var unit = building.Spawn();
            _units.Add(unit);
            AddToQuadrant(unit);
            building.BuildingAttributes.LastTick = timer.Seconds;
        }
    }

    private void AddToQuadrant(GameBaseObject gameBaseObject)
    {
        var qudX = (int) Math.Round(gameBaseObject.BaseObjectAttribute.XPosition / 320);
        var qudY = (int) Math.Round(gameBaseObject.BaseObjectAttribute.YPosition / 320);
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
        foreach (var building in _buildings.Where(building => building.BaseObjectAttribute.DoAState == DoAState.Dead)
                     .ToArray())
        {
            _level._tileLevel[
                    new Vector2(building.BaseObjectAttribute.XPosition / 32,
                        building.BaseObjectAttribute.YPosition / 32)]
                .TileAttribute.ContainBuilding = false;
            _level._tileLevel[
                    new Vector2(building.BaseObjectAttribute.XPosition / 32,
                        building.BaseObjectAttribute.YPosition / 32)]
                .TileAttribute.isPlacibleTile = true;
            _buildings.Remove(building);
            _quadrant[building.BaseObjectAttribute.LastQuadrant].Remove(building);
            building.Dispose();
        }
    }
}