using SimpleLayer.GameEngine.Objects.States;

namespace SimpleLayer.GameEngine.Templates.ObjectsTemplate;

public class DwarfTemplate
{
    public float AttackDistance = 5.0f;
    public int CurrentAttackFrame = 1;
    public int CurrentMovingFrame = 1;
    public int Damage = 5;
    public int HealthPoint = 5;
    public int HeightSprite = 32;
    public int MaxAttackFrame = 7;
    public int MaxMovingFrame = 8;

    public MoAState MoAState = MoAState.Moving;

    //TODO Переделать под Json и сделать автоматический конетейнер который будет хранить конкретные реализации базовых игровых типов объектов
    public string TextureName = "dwarf";
    public int WidthSprite = 32;
}