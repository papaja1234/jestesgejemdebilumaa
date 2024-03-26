using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : Singleton<TetrisManager>
{
    public TetrisElement[,] Elements;
    public TetrisGrid grid;
    public bool hasFallingBlock;
    private int width;
    private int height;
    public Vector2 Position;

    public void Update()
    {
        //todo: game logic
    }

    public enum MoveType
    {
        Nothing = 0,
        Down = 0,
        FastDown,
        RotateRight,
        RotateLeft
    };

    public void InitializeGame(int squareWidth, int squareHeight, int x, int y)
    {
        InitializeGrid(squareWidth, squareHeight);
        Elements = new TetrisElement[squareHeight, squareWidth];
        width = squareWidth;
        height = squareHeight;
        Position = new Vector2(x, y);

        for (int i =  0; i < squareWidth;  i++)
        {
            for (int j = 0; j < squareHeight; j++)
            {
                Elements[i, j].blockType = TetrisBlock.TetrisBlockType.Empty;
            }
        }
        // todo: generate part line for interface
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
