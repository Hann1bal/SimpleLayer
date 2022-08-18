using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.Types;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class EconomyGameManagerWorker
{
    private int LastTick;

    /// <summary>
    ///     Управление экономикой игрока
    /// </summary>
    public EconomyGameManagerWorker()
    {
    }

    /// <summary>
    ///     Запускает базовое накопление золота
    /// </summary>
    public void RunJob(ref Player player, Time time, List<Building> buildings)
    {
        RunDefaultMine(player, time);
        if (buildings.Any(b => b.BuildingAttributes.BuildingType == BuildingType.Mine))
            RunMineWork(ref player,
                buildings.Where(b => b.BuildingAttributes.BuildingType == BuildingType.Mine).ToList(), ref time);
    }

    private void RunDefaultMine(Player player, Time time)
    {
        if (time.Seconds - 15 < LastTick) return;
        player.PlayerAttribute.Gold++;
        LastTick = time.Seconds;
    }

    private void RunMineWork(ref Player player, List<Building> Mines, ref Time time)
    {
        foreach (var mine in Mines)
        {
            if (time.Seconds - mine.BuildingAttributes.GoldPerMinute / 60 < mine.BuildingAttributes.LastTick) return;
            player.PlayerAttribute.Gold++;
            LastTick = time.Seconds;
        }
    }
}