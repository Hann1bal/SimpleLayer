using System.Numerics;
using SDL2;
using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class UnitGameLogicManager
{
    private readonly List<Building> _buildings;
    private readonly List<Unit> _playersUnits;

    private readonly Dictionary<Vector2, List<GameBaseObject>> _quadrant;
    // private List<Unit> _playersDeadUnits = new();


    public UnitGameLogicManager(ref List<Unit> playersUnits, ref Dictionary<Vector2, List<GameBaseObject>> quadrant,
        ref List<Building> buildings)
    {
        _playersUnits = playersUnits;
        _quadrant = quadrant;
        _buildings = buildings;
    }

    public void DoJob()
    {
        MoveAllunits();
    }

    private void MoveAllunits()
    {
        foreach (var unit in _playersUnits.ToArray())
        {
            switch (unit.DRect.x + unit.DRect.w)
            {
                case > 3199:
                case < 0:
                    KillUnit(unit);
                    break;
            }

            CheckDistanceToTarget(unit);
            if (unit.IsDead) KillUnit(unit);

            NearestCoords(unit);
            MoveUnit(unit);
            CheckForSwapQuadrant(unit);
        }
    }

    /*Удаляем юнит из всех списков для обработки;
     Помещаем в мусорный и после отдельно очищаем*/
    private void KillUnit(Unit unit)
    {
        _quadrant[unit.LastQuadrant].Remove(unit);
        _playersUnits.Remove(unit);
        unit.Target = null;
        unit.Dispose();
    }

    private void MoveUnit(Unit unit)
    {
        if (unit.TargetDistance == 0) return;
        unit.CurrentXSpeed = (float) (unit.Target.XPosition - unit.XPosition) / unit.TargetDistance;
        unit.CurrentYSpeed = (float) (unit.Target.YPosition - unit.YPosition) / unit.TargetDistance;
        unit.XPosition += unit.Accelaration * 2 * (int) Math.Round(unit.CurrentXSpeed);
        unit.YPosition += unit.Accelaration * 2 * (int) Math.Round(unit.CurrentYSpeed);
        unit.DRect.x = unit.XPosition;
        unit.DRect.y = unit.YPosition;
        if (unit.CurrentFrame < unit.MaxFrame) unit.CurrentFrame++;
        else unit.CurrentFrame = 1;
    }

    private void CheckDistanceToTarget(Unit unit)
    {
        foreach (var enemy in _quadrant[unit.LastQuadrant].ToList()
                     .Where(enemy => unit.Team != enemy.Team))
        {
            if (unit.TargetDistance >= unit.AttackDistance)
            {
                unit.Accelaration = 1;
                continue;
            }
            unit.Accelaration = 0;
            if (unit.CurrentAttackFrame < unit.MaxAttackFrame) unit.CurrentAttackFrame++;
            else
            {
                unit.CurrentAttackFrame = 1;
                DoAttack(unit, enemy);
            }
        }
    }

    private void DoAttack(Unit unit, GameBaseObject enemy)
    {
        unit.HealthPoint -= enemy.Damage;
        enemy.HealthPoint -= unit.Damage;
        enemy.IsDead = enemy.HealthPoint == 0;
        unit.IsDead = unit.HealthPoint == 0;
    }

    private void NearestCoords(GameBaseObject unit)
    {
        var minimumDistance = int.MaxValue;
        GameBaseObject nearestTarget = null;
        for (var i = unit.LastQuadrant.X - 1; i <= unit.LastQuadrant.X + 1; i++)
        for (var j = unit.LastQuadrant.Y - 1; j <= unit.LastQuadrant.Y + 1; j++)
            foreach (var enemy in _quadrant[new Vector2(i, j)].ToArray())
            {
                if (unit.Team == enemy.Team || enemy.IsDead) continue;
                var distance = DistanceBetween(unit, enemy);
                if (nearestTarget != null && minimumDistance <= distance) continue;
                nearestTarget = enemy;
                minimumDistance = distance;
            }

        if (nearestTarget == null)
        {
            nearestTarget = _buildings.First(b => b.Team != unit.Team);
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

    /*
     * Проверка местонахождения юнита. 
     */
    private void CheckForSwapQuadrant(GameBaseObject gameBaseObject)
    {
        if (!(Vector2.Distance(gameBaseObject.LastQuadrant,
                new Vector2(gameBaseObject.XPosition / 320, gameBaseObject.YPosition / 320)) > 0)) return;
        DeleteFromQuadrant(gameBaseObject);
        AddToQuadrant(gameBaseObject);
    }

    private void DeleteFromQuadrant(GameBaseObject gameBaseObject)
    {
        _quadrant[gameBaseObject.LastQuadrant].Remove(gameBaseObject);
    }

    private void AddToQuadrant(GameBaseObject gameBaseObject)
    {
        var qudX = gameBaseObject.XPosition / 320;
        var qudY = gameBaseObject.YPosition / 320;
        _quadrant[new Vector2(qudX, qudY)].Add(gameBaseObject);
        gameBaseObject.LastQuadrant = new Vector2(qudX, qudY);
    }
}