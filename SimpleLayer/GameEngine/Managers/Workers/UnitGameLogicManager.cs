using System.Numerics;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;

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

    public void RunJob()
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

            MoveOrAttackTarget(unit);
            if (unit.BaseObjectAttribute.DoAState == DoAState.Dead)
            {
                KillUnit(unit);
                continue;
            }

            NearestCoords(unit);
            MoveUnit(unit);
            CheckForSwapQuadrant(unit);
        }
    }

    /*Удаляем юнит из всех списков для обработки;
     Помещаем в мусорный и после отдельно очищаем*/
    private void KillUnit(Unit unit)
    {
        _quadrant[unit.BaseObjectAttribute.LastQuadrant].Remove(unit);
        _playersUnits.Remove(unit);
        unit.UnitsAttributes.Target = null;
        unit.Dispose();
    }

    private void MoveUnit(Unit unit)
    {
        if (unit.UnitsAttributes.TargetDistance < unit.UnitsAttributes.AttackDistance) return;
        unit.UnitsAttributes.DeltaX = (unit.UnitsAttributes.Target.BaseObjectAttribute.XPosition -
                                       unit.BaseObjectAttribute.XPosition) /
                                      unit.UnitsAttributes.TargetDistance;
        unit.UnitsAttributes.DeltaY = (unit.UnitsAttributes.Target.BaseObjectAttribute.YPosition -
                                       unit.BaseObjectAttribute.YPosition) /
                                      unit.UnitsAttributes.TargetDistance;
        unit.BaseObjectAttribute.XPosition += unit.UnitsAttributes.Accelaration * 4 * unit.UnitsAttributes.DeltaX;
        unit.BaseObjectAttribute.YPosition += unit.UnitsAttributes.Accelaration * 4 * unit.UnitsAttributes.DeltaY;
        unit.DRect.x = (int) Math.Round(unit.BaseObjectAttribute.XPosition);
        unit.DRect.y = (int) Math.Round(unit.BaseObjectAttribute.YPosition);
        if (unit.UnitsAttributes.CurrentMovingFrame < unit.UnitsAttributes.MaxMovingFrame)
            unit.UnitsAttributes.CurrentMovingFrame++;
        else unit.UnitsAttributes.CurrentMovingFrame = 1;
    }

    private void MoveOrAttackTarget(Unit unit)
    {
        if (unit.UnitsAttributes.TargetDistance > unit.UnitsAttributes.AttackDistance)
        {
            unit.UnitsAttributes.Accelaration = 1;
            unit.UnitsAttributes.MoAState = MoAState.Moving;
            return;
        }

        unit.UnitsAttributes.Accelaration = 0;
        unit.UnitsAttributes.MoAState = MoAState.Attacking;
        if (unit.UnitsAttributes.CurrentAttackFrame < unit.UnitsAttributes.MaxAttackFrame)
        {
            unit.UnitsAttributes.CurrentAttackFrame++;
        }
        else
        {
            unit.UnitsAttributes.CurrentAttackFrame = 1;
            DoAttack(unit, unit.UnitsAttributes.Target);
        }
    }


    private void DoAttack(Unit unit, GameBaseObject enemy)
    {
        enemy.BaseObjectAttribute.HealthPoint -= unit.UnitsAttributes.Damage;
        enemy.BaseObjectAttribute.DoAState =
            enemy.BaseObjectAttribute.HealthPoint <= 0 ? DoAState.Dead : DoAState.Alive;
        if (enemy.BaseObjectAttribute.DoAState == DoAState.Dead) unit.UnitsAttributes.Target = null;
    }

    private void NearestCoords(Unit unit)
    {
        var minimumDistance = int.MaxValue;
        GameBaseObject? nearestTarget = null;
        for (var i = unit.BaseObjectAttribute.LastQuadrant.X - 1; i <= unit.BaseObjectAttribute.LastQuadrant.X + 1; i++)
        for (var j = unit.BaseObjectAttribute.LastQuadrant.Y - 1; j <= unit.BaseObjectAttribute.LastQuadrant.Y + 1; j++)
            foreach (var enemy in _quadrant[new Vector2(i, j)].ToArray())
            {
                if (unit.BaseObjectAttribute.Team == enemy.BaseObjectAttribute.Team ||
                    enemy.BaseObjectAttribute.DoAState == DoAState.Dead) continue;
                var distance = DistanceBetween(unit, enemy);
                if (minimumDistance <= distance) continue;
                nearestTarget = enemy;
                minimumDistance = distance;
            }

        if (nearestTarget == null)
        {
            nearestTarget = _buildings.First(b => b.BaseObjectAttribute.Team != unit.BaseObjectAttribute.Team);
            minimumDistance = DistanceBetween(unit, nearestTarget);
        }

        unit.UnitsAttributes.TargetDistance = minimumDistance;
        unit.UnitsAttributes.Target = nearestTarget;
    }

    private static int DistanceBetween(GameBaseObject unit, GameBaseObject target)
    {
        return (int) Math.Round(Vector2.Distance(
            new Vector2(unit.BaseObjectAttribute.XPosition, unit.BaseObjectAttribute.YPosition),
            new Vector2(target.BaseObjectAttribute.XPosition, target.BaseObjectAttribute.YPosition)));
    }

    /*
     * Проверка местонахождения юнита. 
     */
    private void CheckForSwapQuadrant(GameBaseObject gameBaseObject)
    {
        if (!(Vector2.Distance(gameBaseObject.BaseObjectAttribute.LastQuadrant,
                new Vector2(gameBaseObject.BaseObjectAttribute.XPosition / 320,
                    gameBaseObject.BaseObjectAttribute.YPosition / 320)) > 0)) return;
        DeleteFromQuadrant(gameBaseObject);
        AddToQuadrant(gameBaseObject);
    }

    private void DeleteFromQuadrant(GameBaseObject gameBaseObject)
    {
        _quadrant[gameBaseObject.BaseObjectAttribute.LastQuadrant].Remove(gameBaseObject);
    }

    private void AddToQuadrant(GameBaseObject gameBaseObject)
    {
        var qudX = (int) Math.Round(gameBaseObject.BaseObjectAttribute.XPosition) / 320;
        var qudY = (int) Math.Round(gameBaseObject.BaseObjectAttribute.YPosition) / 320;
        _quadrant[new Vector2(qudX, qudY)].Add(gameBaseObject);
        gameBaseObject.BaseObjectAttribute.LastQuadrant = new Vector2(qudX, qudY);
    }
}