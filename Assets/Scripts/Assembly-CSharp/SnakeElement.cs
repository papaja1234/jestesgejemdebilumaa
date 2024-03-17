using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeElement : ClickInteraction
{
    public enum SnakeElementType
    {
        Empty,
        SnakeBody,
        SnakeHead,
        SnakeTail,
        Egg
    }

    protected override void Start()
    {
        // Dear Goggs, dew it. -- Anstro Pleuton
    }

    public void UpdateDisplay()
    {
        // Dear Goggs, you know the drill. -- Anstro Pleuton
    }
}
