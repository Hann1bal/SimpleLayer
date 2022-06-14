namespace SimpleLayer.Objects;

public class Unit : GameBaseObject, IDisposable
{
    private const int Speed = 1;
    private IntPtr _renderer;

    public int MaxAttackFrame;

    // public float CurrentYSpeed = 1;
    public int MaxFrame;

    public Unit(string textureName, int xPos, int yPos,
        int healtPpoint, int team, int damage, int maxFrame, int maxAttackFrame) :
        base(textureName, xPos, yPos, healtPpoint, team, false, damage)
    {
        MaxFrame = maxFrame;
        MaxAttackFrame = maxAttackFrame;
    }

    public new void Dispose()
    {
        _renderer = IntPtr.Zero;
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}