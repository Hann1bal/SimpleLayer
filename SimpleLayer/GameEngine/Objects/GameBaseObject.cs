using System.Numerics;
using SDL2;

namespace SimpleLayer.Objects;

public class GameBaseObject : IGameBaseObject
{
    public readonly int Damage;
    public int CurrentFrame = 1;
    public int CurrentAttackFrame = 1;
    public float CurrentXSpeed = 0;
    public float CurrentYSpeed = 0;
    public SDL.SDL_Rect DRect;
    public int HealthPoint;
    public bool IsBuildng;
    public bool IsDead;
    public Vector2 LastQuadrant;
    public SDL.SDL_Rect SRect;
    public GameBaseObject Target;
    public int TargetDistance;
    public readonly int AttackDistance = 5;
    public int Team;
    public string TextureName;
    public int XPosition, YPosition;
    public int Accelaration = 1;


    public GameBaseObject(string textureName, int xPos, int yPos,
        int healthPoint, int team, bool isBuildng, int damage = 0)
    {
        Team = team;
        IsBuildng = isBuildng;
        Damage = damage;
        HealthPoint = healthPoint;
        XPosition = xPos;
        YPosition = yPos;
        TextureName = textureName;
        SRect.h = 210;
        SRect.w = 210;
        DRect.x = XPosition;
        DRect.y = YPosition;
        DRect.w = SRect.w / 10;
        DRect.h = SRect.h / 10;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(DRect);
        GC.SuppressFinalize(SRect);
        GC.Collect(GC.MaxGeneration);
    }


    ~GameBaseObject()
    {
        Dispose();
    }
}