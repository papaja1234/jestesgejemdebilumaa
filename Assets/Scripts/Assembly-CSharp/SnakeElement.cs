using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeElement : ClickInteraction/*Click() should be unused */
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

    public int BGtype;
    public SnakeBlockType blockType;
    public INSerializedSprite display;
    protected override void Start()
    {
        // Dear Goggs, dew it. -- Anstro Pleuton
        GetComponent<INSerializedSprite>().Reload("BG_"+(BGtype % 2).ToString() );
        display = transform.Find("Display").gameObject.GetComponent<INSerializedSprite>();
    }

    public void UpdateDisplay()
    {
        // Dear Goggs, you know the drill. -- Anstro Pleuton
        if (blockType!=SnakeBlockType.Empty)
        {
            display.Reload($"S_{blockType}");
        }
        else
        {
            display.Reload("");
        }
    }
}
