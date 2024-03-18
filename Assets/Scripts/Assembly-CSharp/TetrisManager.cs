using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : Singleton<TetrisManager>
{
    public TetrisElement[,] Elements;

    private int width;

    private int height;

    private List<List<Vector2Int>> shapes = new List<List<Vector2Int>> {
        new List<Vector2Int> { new Vector2Int(1, 2) }
    };

    public Vector2 Position;

    public enum ChangeType
    {
        Left,
        Right,
        Rotate,
        FastDownward
    }

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
                Elements[i, j].blockType = TetrisElement.TetrisBlockType.Empty;
            }
        }
    }

    private void Change(ChangeType changeType)
    {

    }
}
