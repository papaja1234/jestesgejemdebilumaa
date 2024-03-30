public class PieceType
{
    public readonly Arr2D<bool>[] Rotations;
    public readonly TileType Type;

    public PieceType(Arr2D<bool>[] rotations, TileType type)
    {
        this.Rotations = rotations;
        this.Type = type;
    }
}