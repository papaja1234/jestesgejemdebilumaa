using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct TetrisBlock
{
    public enum TetrisBlockType
    {
        Empty = 0,
        I = 1, // Cyan
        J = 2, // Blue
        L = 3, // Orange
        O = 4, // Yellow
        S = 5, // Green
        T = 6, // Purple
        Z = 7  // Red
    }

    public Vector2Int offset;

    public TetrisBlockType blockType;

    public TetrisBlock(TetrisBlockType blockType)
    {
        this.blockType = blockType;
        offset = Vector2Int.zero;
        //fix errors
    }

    public TetrisBlock(int blockType)
    {
        this.blockType = (TetrisBlockType)blockType;
        offset = Vector2Int.zero;
    }
}

class TetrisFallingTetromino
{
 

    // The array being 2D is redundant...
    public TetrisBlock[,] tetromino;

    public int width;

    public int height;

    public TetrisGrid parentGrid;

    public Vector2Int position;

    TetrisFallingTetromino(TetrisGrid grid, TetrisBlock.TetrisBlockType blockType)
    {
        parentGrid = grid;

        switch (blockType)
        {
            case TetrisBlock.TetrisBlockType.Empty: // 0
                tetromino = new TetrisBlock[,] {};
                break;
            case TetrisBlock.TetrisBlockType.I: // 
                tetromino = new TetrisBlock[,] {
                    { new TetrisBlock(1), new TetrisBlock(1), new TetrisBlock(1), new TetrisBlock(1) }
                };
                width = 4;
                height = 1;
                break;
            case TetrisBlock.TetrisBlockType.J: // 2
                tetromino = new TetrisBlock[,] {
                    { new TetrisBlock(2), new TetrisBlock(0), new TetrisBlock(0) },
                    { new TetrisBlock(2), new TetrisBlock(2), new TetrisBlock(2) }
                };
                width = 3;
                height = 2;
                break;
            case TetrisBlock.TetrisBlockType.L: // 3
                tetromino = new TetrisBlock[,] {
                    { new TetrisBlock(0), new TetrisBlock(0), new TetrisBlock(3) },
                    { new TetrisBlock(3), new TetrisBlock(3), new TetrisBlock(3) }
                };
                width = 3;
                height = 2;
                break;
            case TetrisBlock.TetrisBlockType.O: // 4
                tetromino = new TetrisBlock[,] {
                    { new TetrisBlock(4), new TetrisBlock(4) },
                    { new TetrisBlock(4), new TetrisBlock(4) }
                };
                width = 2;
                height = 2;
                break;
            case TetrisBlock.TetrisBlockType.S: // 5
                tetromino = new TetrisBlock[,] {
                    { new TetrisBlock(0), new TetrisBlock(5), new TetrisBlock(5) },
                    { new TetrisBlock(5), new TetrisBlock(5), new TetrisBlock(0) }
                };
                width = 3;
                height = 2;
                break;
            case TetrisBlock.TetrisBlockType.T: // 6
                tetromino = new TetrisBlock[,] {
                    { new TetrisBlock(0), new TetrisBlock(6), new TetrisBlock(0) },
                    { new TetrisBlock(6), new TetrisBlock(6), new TetrisBlock(6) }
                };
                width = 3;
                height = 2;
                break;
            case TetrisBlock.TetrisBlockType.Z: // 7
                tetromino = new TetrisBlock[,] {
                    { new TetrisBlock(7), new TetrisBlock(7), new TetrisBlock(0) },
                    { new TetrisBlock(0), new TetrisBlock(7), new TetrisBlock(7) }
                };
                width = 3;
                height = 2;
                break;
        }

        // Calculate offsets
        // It's easy only because it is 2D array
        // That might be the only use case of it being 2D array
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tetromino[i, j].offset = new Vector2Int(i, j);
            }
        }
    }

    private Vector2Int RotatePoint(Vector2Int point, Vector2Int center, bool rightwords)
    {
        float angle = rightwords ? -Mathf.PI / 2.0f : Mathf.PI / 2.0f;
        int cos = (int)Mathf.Cos(angle);
        int sin = (int)Mathf.Sin(angle);

        point -= center;
        Vector2Int newPoint = new Vector2Int(
            point.x * cos - point.y * sin,
            point.x * sin + point.y * cos//MATRIX
        );

        return newPoint + center;
    }

    public void Rotate(bool rightwords)
    {
        // Rotate on rough center
        Vector2Int roughCenter = new Vector2Int(width / 2, height / 2);

        TetrisBlock[,] newTetromino = new TetrisBlock[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                newTetromino[i, j] = tetromino[i, j];
                newTetromino[i, j].offset = RotatePoint(tetromino[i, j].offset, roughCenter, rightwords);
            }
        }

        // Check collision against walls
        bool collidedWithWalls = false;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2Int vec1 = newTetromino[i, j].offset + position;
                Vector2Int vec2 = new Vector2Int(parentGrid.width, parentGrid.height);
                if ( (vec1.x>=vec2.x && vec1.y > vec2.y)   || vec1 is { x: < 0, y: < 0 })
                {
                    collidedWithWalls = true;
                    break;
                }
            }
        }

        if (!collidedWithWalls)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tetromino[i, j].offset = newTetromino[i, j].offset;
                }
            }
        }

        // No. Don't swap it because it will cause errors
        // (width, height) = (height, width);
    }

    // Check collision of the next move before moving and permanently placing the tetromino
    public bool CheckCollision()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < parentGrid.width; k++)
                {
                    for (int l = 0; l < parentGrid.height; l++)
                    {
                        if (parentGrid.blocks[k, l].blockType != TetrisBlock.TetrisBlockType.Empty && tetromino[i, j].blockType != TetrisBlock.TetrisBlockType.Empty)
                            // Hyper nesting moment
                            return true;
                    }
                }
            }
        }
        //add return
        return false;
    }

    public void MoveDown()
    {
        // Yeap
        position.y++;
    }
}

public class TetrisGrid
{
    
    // You might need to flip the y axis when rednering
    public TetrisBlock[,] blocks;

    public int width;

    public int height;

    public TetrisGrid(int squareWidth, int squareHeight)
    {
        blocks = new TetrisBlock[squareWidth, squareHeight];
        width = squareWidth;
        height = squareHeight;
    }

    public List<int> GetRemovableRows()
    {
        List<int> removableRows = new List<int>();
        for (int j = 0; j < height; j++)
        {
            bool fullRow = true;
            for (int i = 0; i < width; i++)
            {
                if (blocks[i, j].blockType == TetrisBlock.TetrisBlockType.Empty)
                {
                    fullRow = false;
                    break;
                }
            }
            if (fullRow)
            {
                removableRows.Append(j);
            }
        }
        return removableRows;
    }

    public void RemoveRemovableRows(List<int> removableRows)
    {
        foreach (int removableRow in removableRows)
        {
            for (int j = 0; j < removableRow; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    blocks[i, j + 1] = blocks[i, j];
                    blocks[i, j] = new TetrisBlock(){blockType = TetrisBlock.TetrisBlockType.Empty};
                }
            }
        }
    }
}

