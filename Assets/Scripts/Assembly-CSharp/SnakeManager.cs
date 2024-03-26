using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SnakeManager : Singleton<SnakeManager>
{
    public SnakeElement[,] Elements;

    private int width;

    private int height;
    //public KeyListener keyListener;
    //not using this since it's a singleton<keyListener>
    public KeyCode lastInput;
    private Vector2Int EggPosition;

    private int frameCount = 0;
    private const int moveInterval = 2;

    public void Update()
    {
        GetInput();//set lastInput
        frameCount++;
        if (frameCount % moveInterval == 0) 
        {
            UpdateStatus(ToTurnType(lastInput));
        }
    }

    public void GetInput()
    {
        /*
            KeyCode.W,
            KeyCode.A,
            KeyCode.S,
            KeyCode.D,
            KeyCode.LeftArrow,
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.RightArrow*/
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            lastInput = KeyCode.UpArrow;   //only use arrow keys for conditions
            return;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            lastInput = KeyCode.LeftArrow; //only use arrow keys for conditions
            return;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            lastInput = KeyCode.DownArrow; //only use arrow keys for conditions
            return;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            lastInput = KeyCode.RightArrow;//only use arrow keys for conditions
            return;
        }

        lastInput = KeyCode.None;
    }

    public void Awake()
    {/*
        keyListener = Instance.gameObject.AddComponent<KeyListener>();
        keyListener.m_hotkeys = new List<KeyCode>()
        {
            KeyCode.W,
            KeyCode.A,
            KeyCode.S,
            KeyCode.D,
            KeyCode.LeftArrow,
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.RightArrow
        };
        //not using this class since it's a singleton
        keyListener.enabled = true;*/
        SetAsPersistant();
    }

    // Tail -> First element
    // Body -> Middle all elements
    // Head -> Last element
    private List<Vector2Int> Snake;

    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    };

    private Direction direction;

    public Vector2 Position;

    public TurnType ToTurnType(KeyCode input)
    {
        return input switch
        {
            KeyCode.LeftArrow => TurnType.Left,
            KeyCode.RightArrow => TurnType.Right,
            KeyCode.UpArrow => TurnType.Up,
            KeyCode.DownArrow => TurnType.Down,
            _ => TurnType.None
        };
    }

    public enum TurnType
    {
        Left,
        Right,
        Up,
        Down,

        // Make sure other properties are after the first four
        None // None because it should update status every tick (not every frame)
    }

    public void InitializeGame(int squareWidth, int squareHeight, int x, int y)
    {
        Elements = new SnakeElement[squareWidth, squareHeight];
        width = squareWidth;
        height = squareHeight;
        Position = new Vector2(x, y);

        // Be sure to have at least one element
        Snake = new List<Vector2Int>(1);

        NewEggPosition();
    }

    // Call this before update status?
    public bool CheckWon()
    {
        // More than or equal to, idk why
        return Snake.Count >= (width-1) * (height-1) && Snake.Count > 3;
    }

    public int SnakeLength()
    {
        return Snake.Count;
    }

    // Call this to update element display
    public void UpdateElements()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Elements[i, j].blockType = SnakeElement.SnakeBlockType.Empty;
            }
        }
        foreach (var snake in Snake)
        {
            Elements[snake.x, snake.y].blockType = SnakeElement.SnakeBlockType.SnakeBody;
        }
        if (Snake[0] != Snake[Snake.Count - 1])
        {
            Elements[Snake[0].x, Snake[0].y].blockType = SnakeElement.SnakeBlockType.SnakeTail;
            Elements[Snake[Snake.Count - 1].x, Snake[Snake.Count - 1].y].blockType = SnakeElement.SnakeBlockType.SnakeHead;
        }
        else
        {
            Elements[Snake[0].x, Snake[0].y].blockType = SnakeElement.SnakeBlockType.SnakeSoul;
        }
        Elements[EggPosition.x, EggPosition.y].blockType = SnakeElement.SnakeBlockType.Egg;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Elements[i, j].UpdateDisplay();
            }
        }
    }

    public void UpdateStatus(TurnType turnType)
    {
        // Dear Goggs... ^-^
        if (CheckWon())
        {
            //Play Win-Animation
        }
        else
        {
            Move(turnType);
            UpdateElements();
        }
    }

    private void NewEggPosition()
    {
        // Prevent infinite loop or crash
        if (CheckWon())
        {
            return;
        }
        EggPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        foreach (var snake in Snake)
        {
            if (snake == EggPosition)
            {
                NewEggPosition();
            }
        }
    }

    // Move the snake
    // Return true if snake hit itself or when it hit the border
    private bool Move(TurnType turnType)
    {
        if (turnType != TurnType.None)
        {
            // Make sure the snake direction doesn't change to going backwards
            if ((direction == Direction.Left && turnType != TurnType.Right) ||
                (direction == Direction.Right && turnType != TurnType.Left) ||
                (direction == Direction.Up && turnType != TurnType.Down) ||
                (direction == Direction.Down && turnType != TurnType.Up))
            {
                direction = (Direction)turnType;
            }
        }

        Vector2Int snakeTail = Snake[0];
        Vector2Int snakeHead = Snake[Snake.Count - 1];

        switch (direction)
        {
            case Direction.Left:
                if (snakeTail.x > 0) snakeTail = snakeHead + new Vector2Int(-1, 0);
                else return true;
                break;
            case Direction.Right:
                if (snakeTail.x < width - 1) snakeTail = snakeHead + new Vector2Int(1, 0);
                else return true;
                break;
            case Direction.Up:
                if (snakeTail.y < height - 1) snakeTail = snakeHead + new Vector2Int(0, 1);
                else return true;
                break;
            case Direction.Down:
                if (snakeTail.y > 0) snakeTail = snakeHead + new Vector2Int(0, -1);
                else return true;
                break;
        }
        // NOTE: snakeTail is now Head

        // Check if the snake hit itself
        foreach (var snake in Snake)
        {
            if (snake == snakeTail)
                return true;
        }

        Snake.Add(snakeTail);

        if (snakeTail == EggPosition)
        {
            NewEggPosition();
        }
        else
        {
            Snake.RemoveAt(0);
        }
        return false;
    }
}
