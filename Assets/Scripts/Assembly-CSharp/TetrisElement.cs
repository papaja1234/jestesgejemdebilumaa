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

    //public TileType blockType;
    //public FallingPiece parentTetromino;

    private INSerializedSprite sprite;
    public Vector2Int boardPos = Vector2Int.zero;

    public override void Awake()
    {
        base.Awake();
        this.sprite = GetComponent<INSerializedSprite>();
    }

    protected override void OnTouch(bool hasPosition, Vector3 touchPosition, GuiManager.Pointer pointerInfo = default)
    {
        FallingPiece f = TetrisManager.Instance.Board.Falling;
        //TODO: IMPLEMENT THIS FOR MOBILE MOVEMENT (boardpos to get this element's pos)
    }

    protected override void Start()
    {
        base.Start();
        // Dear Goggs, dew it. -- Anstro Pleuton
    }

    public void UpdateDisplay(TileType blockType)
    {
        // Dear Goggs, you know the drill. -- Anstro Pleuton
        // Waiting for assets...done
        // Waiting for asset installation
        this.sprite.Reload($"T_{blockType.ToString()}");
    }
}
