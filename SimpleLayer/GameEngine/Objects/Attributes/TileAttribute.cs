namespace SimpleLayer.GameEngine.Objects.Attributes;

public class TileAttribute
{
    public bool _isMoveble = true;
    public bool ContainBuilding = false;
    public bool isPlacibleTile = false;
    public IntPtr _texture { get; init; }
    public int Id { get; set; }
}