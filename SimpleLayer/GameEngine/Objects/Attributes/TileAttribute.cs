namespace SimpleLayer.GameEngine.Objects.Attributes;

public class TileAttribute
{
    public IntPtr _texture { get; init; }
    public bool _isMoveble = true;
    public bool ContainBuilding = false;
    public int Id { get; set; }
    public bool isPlacibleTile = false;
}