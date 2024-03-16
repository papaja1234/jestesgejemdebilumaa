using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSweeperElement : ClickInteraction
{
    // What?
    public int ForeGroundType = 0;
    public enum MineSweeperBlockType
    {
        Empty = 1,
        Bomb = 2,
    }

    public MineSweeperBlockType blockType;

    public bool isOpened;

    public bool isFlagged;

    public bool isQuestioned;

    public int digit;

    protected override void Start()
    {
        GetComponent<INSerializedSprite>().Reload(ForeGroundType == 0?"MS_Default":"MS_DefaultVariation");
    }
}
