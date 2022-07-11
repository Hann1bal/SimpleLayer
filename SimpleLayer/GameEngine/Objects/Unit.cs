using SimpleLayer.Objects.States;

namespace SimpleLayer.Objects;

public class Unit : GameBaseObject, IDisposable
{
    public UnitsAttributes UnitsAttributes;

    public Unit(string textureName, int xPos, int yPos,
        int healthPoint, int team, int damage, int maxMovingFrame, int maxAttackFrame) :
        base(textureName, xPos, yPos, healthPoint, team, ObjectType.Unit)
    {
        UnitsAttributes = new UnitsAttributes
        {
            Damage = damage, CurrentAttackFrame = 1, CurrentMovingFrame = 1, AttackDistance = 5.0f,
            MoAState = MoAState.Moving, MaxMovingFrame = maxMovingFrame,
            MaxAttackFrame = maxAttackFrame
        };
    }

    public new void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}