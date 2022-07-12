using System.Numerics;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;

namespace SimpleLayer.GameEngine.Objects.Attributes;

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