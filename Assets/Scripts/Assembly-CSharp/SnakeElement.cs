using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeElement : ClickInteraction
{
    public enum SnakeBlockType
    {
        Empty,
        SnakeBody,
        SnakeHead,
        SnakeTail,
        SnakeSoul, // Snake head and tail are same
        Egg
    }

    public SnakeBlockType blockType;

    protected override void Start()
    {
        // Dear Goggs, dew it. -- Anstro Pleuton
    }

    public void UpdateDisplay()
    {
        // Dear Goggs, you know the drill. -- Anstro Pleuton
    }
}
