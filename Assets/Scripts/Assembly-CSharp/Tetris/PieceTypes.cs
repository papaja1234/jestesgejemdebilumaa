using System;

public static class PieceTypes
{
    public static readonly PieceType I =
        ShapeFromString("0000-1111-0000-0000=0010-0010-0010-0010=0000-0000-1111-0000=0100-0100-0100-0100", TileType.I);
    public static readonly PieceType J =
        ShapeFromString("100-111-000=011-010-010=000-111-001=010-010-110", TileType.J);
    public static readonly PieceType L =
        ShapeFromString("001-111-000=010-010-011=000-111-100=110-010-010", TileType.L);
    public static readonly PieceType O =
        ShapeFromString("11-11", TileType.O);
    public static readonly PieceType S =
        ShapeFromString("011-110-000=010-011-001=000-011-110=100-110-010", TileType.S);
    public static readonly PieceType T =
        ShapeFromString("010-111-000=010-011-010=000-111-010=010-110-010", TileType.T);
    public static readonly PieceType Z =
        ShapeFromString("110-011-000=001-011-010=000-110-011=010-110-100", TileType.Z);
    public static readonly PieceType[] All = new PieceType[] { I, J, L, O, S, T, Z };
    
    private static PieceType ShapeFromString(string s, TileType t)
    {
        string[] rotations = s.Split("=", StringSplitOptions.RemoveEmptyEntries);
        Arr2D<bool>[] shapeData = new Arr2D<bool>[rotations.Length];

        for (int i = 0; i < rotations.Length; i++)
        {
            string[] lines = rotations[i].Split("-", StringSplitOptions.RemoveEmptyEntries);
            Arr2D<bool> b = new Arr2D<bool>(lines[0].Length, lines.Length, false);
            
            for (int y = 0; y < lines.Length; y++)
            {
                string line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    //invert y
                    b[x, b.Height - y - 1] = line[x] == '1';
                }
            }

            shapeData[i] = b;
        }
        
        return new PieceType(shapeData, t);
    }
}