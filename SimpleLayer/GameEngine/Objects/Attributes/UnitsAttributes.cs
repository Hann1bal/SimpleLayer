using SimpleLayer.GameEngine.Objects.States;

namespace SimpleLayer.GameEngine.Objects.Attributes;

public struct UnitsAttributes
{
    public int Damage { get; set; }
    public int Accelaration { get; set; }

    public int CurrentMovingFrame { get; set; }
    public int CurrentAttackFrame { get; set; }
    public int MaxAttackFrame { get; init; }
    public int MaxMovingFrame { get; init; }

    public GameBaseObject Target { get; set; }
    public float TargetDistance { get; set; }
    public float AttackDistance { get; init; }
    public float DeltaX { get; set; }
    public float DeltaY { get; set; }
    public MoAState MoAState { get; set; }
}