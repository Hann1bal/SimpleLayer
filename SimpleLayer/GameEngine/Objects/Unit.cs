namespace SimpleLayer.Objects;

public class Unit : GameBaseObject, IDisposable
{
    public readonly int MaxAttackFrame;
    public readonly int MaxFrame;

    public Unit(string textureName, int xPos, int yPos,
        int healtPpoint, int team, int damage, int maxFrame, int maxAttackFrame) :
        base(textureName, xPos, yPos, healtPpoint, team, false, damage)
    {
        MaxFrame = maxFrame;
        MaxAttackFrame = maxAttackFrame;
    }

    public new void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}