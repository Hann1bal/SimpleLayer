using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class EconomyGameManagerWorker
{
    private int LastTick;
    public List<Building> Mines;

    /// <summary>
    ///     Управление экономикой игрока
    /// </summary>
    public EconomyGameManagerWorker()
    {
    }

    /// <summary>
    ///     Запускает базовое накопление золота
    /// </summary>
    public void RunJob(Player player, Time time)
    {
        RunDefaultMine(player, time);
        // RunMineWork(ref Gold, Mines, ref time);
    }

    private void RunDefaultMine(Player player, Time time)
    {
        if (time.Seconds - 15 < LastTick) return;
        player.PlayerAttribute.Gold++;
        LastTick = time.Seconds;
    }

    private void RunMineWork(ref int Gold, List<Building> Mines, ref Time time)
    {
        foreach (var mine in Mines)
        {
            if (time.Seconds - 15 < mine.BuildingAttributes.LastTick) return;
            Gold++;
            LastTick = time.Seconds;
        }
    }
}