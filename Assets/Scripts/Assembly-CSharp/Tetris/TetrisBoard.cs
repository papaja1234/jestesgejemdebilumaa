using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TetrisBoard
{
    public readonly Arr2D<TileType> Data;
    public FallingPiece Falling;

    private PieceType pickHistory;

    public TetrisBoard()
    {
        //fake tiles for out of bounds
        this.Data = new Arr2D<TileType>(10, 20, TileType.O);
        this.pickHistory = null;
        RollNextFalling();
    }

    public void Tick()
    {
        if (this.Falling.Tick())
        {
            //solidify
            for (int y = 0; y < this.Falling.CurrentShape.Height; y++)
            {
                for (int x = 0; x < this.Falling.CurrentShape.Width; x++)
                {
                    if (this.Falling.CurrentShape[x, y])
                    {
                        this.Data[x + this.Falling.Position.x, y + this.Falling.Position.y] = this.Falling.Type.Type;
                    }
                }
            }
            
            //clear rows
            ClearFilledRows();
            
            //next
            RollNextFalling();
        }
    }
    
    private void ClearFilledRows()
    {
        for (int y = this.Data.Height - 1; y >= 0; y--)
        {
            bool rowFull = true;
            for (int x = 0; x < this.Data.Width; x++)
                if (this.Data[x,y] == TileType.Empty)
                {
                    rowFull = false;
                    break;
                }

            if (rowFull)
            {
                ShiftDown(y);
            }
        }
    }
    
    private void ShiftDown(int y)
    {
        //clear that level
        Array.Fill(this.Data.Raw, TileType.Empty, y * this.Data.Width, this.Data.Width);
        
        //move all above down
        for (int my = y+1; my < this.Data.Height; my++)
        {
            Array.Copy(this.Data.Raw, my * this.Data.Width, this.Data.Raw, (my - 1) * this.Data.Width, this.Data.Width);
        }
    }

    private void RollNextFalling()
    {
        //NES tetris randomizer
        PieceType t = RandPieceType();
        if (t == this.pickHistory) t = RandPieceType();
        this.pickHistory = t;
        this.Falling = new FallingPiece(new Vector2Int(this.Data.Width/2-t.Rotations[0].Width/2, this.Data.Height - 1), this, t);
    }

    private PieceType RandPieceType()
    {
        return PieceTypes.All[Random.Range(0, PieceTypes.All.Length)];
    }
}