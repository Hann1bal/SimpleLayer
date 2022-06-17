using System.Numerics;

namespace SimpleLayer.Objects;

[Serializable]
public struct Event
{
    public int Id { get; set; }
    public string TargetType { get; set; }
    public string TargetName { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}