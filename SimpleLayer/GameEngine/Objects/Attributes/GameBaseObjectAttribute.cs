using System.Numerics;
using SDL2;
using SimpleLayer.Objects.States;

namespace SimpleLayer.Objects;

public struct GameBaseObjectAttribute
{
    public string TextureName { get; set; }
    public int HealthPoint { get; set; }
    public int Team { get; set; }
    public DoAState DoAState { get; set; }
    public ObjectType ObjectType { get; init; }
    public Vector2 LastQuadrant;
    public float XPosition, YPosition;
}