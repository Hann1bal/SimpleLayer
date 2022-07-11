using System.Numerics;
using SimpleLayer.GameEngine.Managers.Workers;
using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine;

public class GameLogicManager
{
    private static GameLogicManager _gameLogicManager;
    private readonly List<Building> _buildings;
    private readonly Level _level;
    private readonly Dictionary<Vector2, List<GameBaseObject>> _quadrant = new();
    private readonly Stack<Event> _receiveEvents;
    private readonly List<Unit> _units;
    private readonly EconomyGameManagerWorker _economyGameManagerWorker;
    private Stack<Event> _events;
    public BuildGameLogicWorker BuildingWorker;
    public UnitGameLogicManager UnitWorker;

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
        _economyGameManagerWorker = new EconomyGameManagerWorker();
    }

    public static GameLogicManager GetInstance(ref List<Building> buildings, ref List<Unit> playersUnits,
        ref Stack<Event> events, ref Stack<Event> receiveEvents, ref Level level)
    {
        if (_gameLogicManager != null) return _gameLogicManager;
        return _gameLogicManager =
            new GameLogicManager(ref buildings, ref playersUnits, ref events, ref receiveEvents, ref level);
    }


    public void RunManager(object? o)
    {
        Array? argArray = new object[2];
        argArray = (Array) o!;
        var player = (Player) argArray.GetValue(1)!;
        var time = (Time) argArray.GetValue(0)!;
        BuildingWorker.RunJob(time);
        UnitWorker.RunJob();
        _economyGameManagerWorker.RunJob(player, time);
    }

    private void InitQuadrant()
    {
        for (var i = -1; i <= 10; i++)
        for (var j = -1; j <= 10; j++)
            _quadrant.Add(new Vector2(i, j), new List<GameBaseObject>());
    }
}