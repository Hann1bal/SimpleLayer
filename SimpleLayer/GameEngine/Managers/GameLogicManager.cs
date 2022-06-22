using System.Numerics;
using SimpleLayer.GameEngine.Managers.Workers;
using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine;

public class GameLogicManager
{
    private static GameLogicManager _gameLogicManager;
    private readonly List<Building> _buildings;
    private readonly Dictionary<Vector2, List<GameBaseObject>> _quadrant = new();
    private readonly List<Unit> _units;
    public Building BuildingBase;
    public Building BuildingBase2;
    public BuildGameLogicWorker BuildingWorker;
    public UnitGameLogicManager UnitWorker;
    private Stack<Event> _events;
    private Level _level;

    private Stack<Event> _receiveEvents;

    private GameLogicManager(ref List<Building> buildings, ref List<Unit> playersUnits, ref Stack<Event> events,
        ref Stack<Event> receiveEvents, ref Level level)
    {
        _buildings = buildings;
        _units = playersUnits;
        _events = events;
        _level = level;
        _receiveEvents = receiveEvents;
        InitQuadrant();
        BuildingWorker = new BuildGameLogicWorker(ref _buildings, ref _quadrant, ref _units, ref events,
            ref _receiveEvents, ref _level);
        BuildingWorker.InitPlayerBase();
        UnitWorker = new UnitGameLogicManager(ref _units, ref _quadrant, ref buildings);
    }

    public static GameLogicManager GetInstance(ref List<Building> buildings, ref List<Unit> playersUnits,
        ref Stack<Event> events, ref Stack<Event> receiveEvents, ref Level level)
    {
        if (_gameLogicManager != null) return _gameLogicManager;
        return _gameLogicManager =
            new GameLogicManager(ref buildings, ref playersUnits, ref events, ref receiveEvents, ref level);
    }


    public void RunManager()
    {
        BuildingWorker.DoJob();
        UnitWorker.DoJob();
    }


    private void InitQuadrant()
    {
        for (var i = -1; i <= 10; i++)
        for (var j = -1; j <= 10; j++)
            _quadrant.Add(new Vector2(i, j), new List<GameBaseObject>());
    }
}