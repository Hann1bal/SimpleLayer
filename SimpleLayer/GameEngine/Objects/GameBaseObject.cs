using System.Numerics;
using SDL2;
using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class GameBaseObject : IGameBaseObject
{
    public int XPosition, YPosition;
    public SDL.SDL_Rect SRect;
    public SDL.SDL_Rect DRect;
    public int HealthPoint;
    public string TextureName;
    public Vector2 LastQuadrant;
    public GameBaseObject Target;
    public int TargetDistance;
    public int Team;
    public bool IsDead;
    public readonly int Damage;

    public GameBaseObject(string textureName, int xPos, int yPos,
        int healthPoint, int team, int damage = 0)
    {
        Team = team;
        Damage = damage;
        HealthPoint = healthPoint;
        XPosition = xPos;
        YPosition = yPos;
        TextureName = textureName;
        SRect.h = 300;
        SRect.w = 300;
        DRect.x = XPosition;
        DRect.y = YPosition;
        DRect.w = SRect.w / 10;
        DRect.h = SRect.h / 10;
    }


    ~GameBaseObject()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(TextureName);

        GC.SuppressFinalize(DRect);
        GC.SuppressFinalize(SRect);
        GC.Collect(GC.MaxGeneration);
    }
}