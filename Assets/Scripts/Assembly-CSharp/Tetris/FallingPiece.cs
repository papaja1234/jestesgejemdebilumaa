using System;
using UnityEngine;

public class FallingPiece
{
    private static readonly Vector2Int[][] WallKickNormal =
    {
        // 0 -> 1
        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, 2),
            new Vector2Int(-1, 2)
        },
        // 1 -> 2
        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, -2),
            new Vector2Int(1, -2)
        },
        // 2 -> 3
        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2)
        },
        // 3 -> 0
        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, -2),
            new Vector2Int(-1, -2)
        }
    };

    private static readonly Vector2Int[][] WallKickI =
    {
        // 0 -> 1
        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-2, 1),
            new Vector2Int(1, -2)
        },
        // 1 -> 2
        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(-1, -2),
            new Vector2Int(2, 1)
        },
        // 2 -> 3
        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(2, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(2, -1),
            new Vector2Int(-1, 2)
        },
        // 3 -> 0
        new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(1, 2),
            new Vector2Int(-2, -1)
        }
    };

    public Vector2Int Position;
    private int rotation;

    private readonly TetrisBoard board;
    public readonly PieceType Type;

    public Arr2D<bool> CurrentShape;

    public FallingPiece(Vector2Int pos, TetrisBoard board, PieceType type)
    {
        this.Position = pos;
        this.rotation = 0;
        this.board = board;
        this.Type = type;
        this.CurrentShape = this.Type.Rotations[0];
    }

    public bool Tick()
    {
        Vector2Int newPos = this.Position + Vector2Int.down;
        if (Collides(newPos, this.CurrentShape))
        {
            //turn into blocks
            return true;
        }
        else
        {
            this.Position = newPos;
            return false;
        }
    }

    public void Rotate(int delta)
    {
        int newRot = this.rotation + delta;
        //mathematical modulo
        int r = newRot % this.Type.Rotations.Length;
        newRot = r < 0 ? r + this.Type.Rotations.Length : r;

        Vector2Int[][] table = this.Type.Type == TileType.I ? WallKickI : WallKickNormal;
        Vector2Int[] tests = table[newRot];

        Arr2D<bool> testShape = this.Type.Rotations[newRot];
        
        //wall kick
        for (int i = 0; i < tests.Length; i++)
        {
            Vector2Int offset = tests[i];
            Vector2Int testPos = this.Position + offset;

            if (!Collides(testPos, testShape))
            {
                this.Position = testPos;
                this.rotation = newRot;
                this.CurrentShape = this.Type.Rotations[this.rotation];
                break;
            }
        }
    }

    public void Move(Vector2Int delta)
    {
        Vector2Int newPos = this.Position + delta;
        if (!Collides(newPos, this.CurrentShape)) this.Position = newPos;
    }
    
    private bool Collides(Vector2Int at, Arr2D<bool> shape)
    {
        for (int y = 0; y < shape.Height; y++)
        {
            int globY = y + at.y;
            if (globY >= this.board.Data.Height) continue;
            
            for (int x = 0; x < shape.Width; x++)
            {
                int globX = x + at.x;
                
                if (shape[x, y] && this.board.Data[globX, globY] != TileType.Empty)
                {
                    return true;
                }
            }
        }
        return false;
    }
}