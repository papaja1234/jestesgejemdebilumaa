using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisElement : ClickInteraction
{
    /*
     //discarded
    public enum ETetrisBlockType
    {
        Empty,
        Shape1, // T (0 deg)
        Shape2, // S (flat)
        Shape3, // Z (flat)
        Shape4, // J (-90 deg)
        Shape5, // L (90 deg)
        Shape6, // I (90 deg)
        Shape7  // O (fill)
    }*/

    public TetrisBlock.TetrisBlockType blockType;
    public TetrisFallingTetromino parentTetromino;

    protected override void OnTouch(bool hasPosition, Vector3 touchPosition, GuiManager.Pointer pointerInfo = default)
    {
        if (parentTetromino.isFrozen)
        {
            return;
        }
        parentTetromino.Rotate(pointerInfo.secondaryDown);
    }

    protected override void Start()
    {
        // Dear Goggs, dew it. -- Anstro Pleuton
    }

    public void UpdateDisplay()
    {
        // Dear Goggs, you know the drill. -- Anstro Pleuton
        // Waiting for assets...done
        // Waiting for asset installation
        GetComponent<INSerializedSprite>().Reload($"T_{blockType.ToString()}");
    }
}
