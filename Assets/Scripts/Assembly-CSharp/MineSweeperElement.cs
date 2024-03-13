using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSweeperElement : ClickInteraction
{
    public int ForeGroundType = 0;
    public enum MineSweeperBlockType
    {
        Empty = 1,
        Bomb = 2,
    }

    protected override void Start()
    {
        GetComponent<INSerializedSprite>().Reload(ForeGroundType == 0?"MS_Default":"MS_DefaultVariation");
    }
}
