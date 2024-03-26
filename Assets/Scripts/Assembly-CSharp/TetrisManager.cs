using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : Singleton<TetrisManager>
{
    public TetrisElement[,] Elements;
    public TetrisGrid grid;
    private int width;

    private int height;

    

    public Vector2 Position;

    public enum MoveType
    {
        Nothing,
        FastDown,
        RotateRight,
        RotateLeft
    };

    public void InitializeGame(int squareWidth, int squareHeight, int x, int y)
    {
        Elements = new TetrisElement[squareHeight, squareWidth];
        width = squareWidth;
        height = squareHeight;
        Position = new Vector2(x, y);

        for (int i =  0; i < squareWidth;  i++)
        {
            for (int j = 0; j < squareHeight; j++)
            {
                Elements[i, j].blockType = TetrisElement.ETetrisBlockType.Empty;
            }
        }
    }
    public void InitializeGrid (int squareWidth, int squareHeight)
    {
        grid = new TetrisGrid(squareWidth, squareHeight) { };
        width = squareWidth;
        height = squareHeight;
    }

    private void Move(MoveType moveType)
    {
    }
}
