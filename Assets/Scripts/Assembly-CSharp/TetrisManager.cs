using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TetrisManager : Singleton<TetrisManager>
{
    static (int, int) FindElementIndex<T>(T[,] matrix, T target)
    {
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                if (matrix[row, col].Equals(target))
                {
                    return (row, col);
                }
            }
        }
        return (-1, -1); // Element not found
    }
    public TetrisElement[,] Elements;
    public TetrisElement[,] FrameElements;
    public TetrisGrid grid;
    public TetrisFallingTetromino currentFallingTetromino;
    public bool initalized;
    public bool hasFallingBlock;
    private int width;
    private int height;
    public Vector2 Position;
    private FrameTicker frameTicker = new FrameTicker() { tickInterval = 5, frameCounter = 0 };

    public void Update()
    {
        //calculation part
        if (frameTicker.WillTick() && initalized)
        {
            if (hasFallingBlock)
            {
                currentFallingTetromino.MoveDown();
            }
            else if (!hasFallingBlock)
            {
                currentFallingTetromino = new TetrisFallingTetromino(grid, (TetrisBlock.TetrisBlockType)Random.Range(1,7));
                hasFallingBlock = true;
            }
            //render part
            foreach (TetrisElement tetrisElement in FrameElements)
            {
                (int, int) tuple = FindElementIndex(FrameElements, tetrisElement);
                tetrisElement.blockType = grid.blocks[tuple.Item1,tuple.Item2].blockType;
                tetrisElement.UpdateDisplay();
            }
        }
        frameTicker.Count();
        
        
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

        for (int i = -squareWidth/2; i <= squareWidth/2; i++)
        {
            GameObject border = Instantiate(INRuntimeGameData.Instance.GameData.m_parts.Find(o => o.GetComponent<Pig>() != null ));
            border.GetComponent<BasePart>().transform.position =
                new Vector3(transform.position.x + i, transform.position.y - 1);
            
        }

        //initialize frame element
        FrameElements = new TetrisElement[squareWidth, squareHeight];
        int index = 0;
        foreach (TetrisElement te in FrameElements)
        {
            GameObject gameObject = Instantiate(Singleton<INRuntimeGameData>.Instance.UnlistedPart.Parts[3]);
            TetrisElement tetrisElement = gameObject.GetComponent<TetrisElement>();
            tetrisElement.blockType = TetrisBlock.TetrisBlockType.Empty;
            int row = (int)Math.Floor(index / (double)squareWidth);
            int colomn = index % 7;
            tetrisElement.transform.position =
                new Vector3(transform.position.x + 1 + row, transform.position.y + 1 + colomn);
            FrameElements.SetValue(tetrisElement, row, colomn);
            index++;
        }
        initalized = true;
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
