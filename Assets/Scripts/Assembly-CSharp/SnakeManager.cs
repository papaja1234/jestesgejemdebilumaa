using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : Singleton<SnakeManager>
{
    public SnakeElement[,] Elements;

    private int width;

    private int height;

    private Vector2Int EggPosition;

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

    private Vector2 Position;

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
        return Snake.Count >= width * height;
    }

    public int SnakeLength()
    {
        return Snake.Count;
    }

    public void UpdateStatus(TurnType turnType)
    {
        // Dear Goggs... ^-^
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
