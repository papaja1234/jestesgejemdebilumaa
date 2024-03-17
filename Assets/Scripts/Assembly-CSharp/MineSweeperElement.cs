using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class MineSweeperElement : ClickInteraction
{
    // What? -- Anstro Pleuton
    // ForeGroundType == 0?"MS_Default":"MS_DefaultVariation" -- Goggs
    /// <summary>
    /// Has usage at
    /// <see cref="Start"/>
    /// </summary>
    public int ForeGroundType = 0;

    public int MatX;
    public int MatY;
    private INSerializedSprite DisplayContent;

    public MineSweeperManager parent;

    public enum MineSweeperBlockType
    {
        Empty = 1,
        Bomb = 2,
        Chuck = 3,
        Red = 4,
        Max = 3
    }

    public MineSweeperBlockType blockType;

    public bool isOpened;

    public bool isFlagged;

    public bool isQuestioned;

    // don't display if this is 0
    public int digit;

    private string[] birds = new[] { "MS_Red", "MS_Chuck", "MS_Bomb" };

    private string[] digits = new[]
    {
        "<place holder index 0>",
        "MS_One",
        "MS_Two",
        "MS_Three",
        "MS_Four",
        "MS_Five",
        "MS_Six",
        "MS_Seven",
        "MS_Eight"
    };

    protected override void Start()
    {
        DisplayContent = transform.Find("Display").gameObject.GetComponent<INSerializedSprite>();
    }

    public void Reload()
    {
        gameObject.GetComponent<INSerializedSprite>().Reload(ForeGroundType == 0 ? "MS_Default" : "MS_DefaultVariation");
    }

    protected override void OnTouch(bool hasPosition, Vector3 touchPosition, GuiManager.Pointer pointerInfo)
    {
        //check for input type
        if (pointerInfo.doubleClick || pointerInfo.secondaryDown)
        {
            parent.UpdateStatus(MatX, MatY, MineSweeperManager.SweepType.DoubleClick);
            return;
        }

        parent.UpdateStatus(MatX, MatY, MineSweeperManager.SweepType.LeftClick);
    }

    //set then invoke, or invoke to set?
    /*bool Opened, bool Flagged, bool Questioned, int Digit*/
    //i prefer the former.
    public void UpdateDisplay()
    {
        if (isOpened)
        {
            switch (blockType)
            {
                case MineSweeperBlockType.Bomb:
                    DisplayContent.Reload(birds[Random.Range(0, 2)]);
                    break;
                case MineSweeperBlockType.Empty:
                {
                    GetComponent<INSerializedSprite>().Reload("MS_DefaultBackground");
                    if (digit != 0)
                    {
                        DisplayContent.Reload(digits[digit]);
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else if (isFlagged)
        {
            DisplayContent.Reload("MS_BirdTag");
        }
    }
}