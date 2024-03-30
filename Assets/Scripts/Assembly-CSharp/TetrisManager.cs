using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TetrisManager : Singleton<TetrisManager>
{
    /*static (int, int) FindElementIndex<T>(T[,] matrix, T target)
    {
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                if (ReferenceEquals(target,matrix[row,col]))
                {
                    return (row, col);
                }
            }
        }
        return (-1, -1); // Element not found
    }
    public TetrisElement[,] Elements;
    public TetrisElement[,] FrameElements;
    public TetrisGrid grid;
    public TetrisFallingTetromino currentFallingTetromino;
    public bool initalized;
    public bool hasFallingBlock;
    private int width;
    private int height;
    public Vector2 Position;
    private FrameTicker frameTicker = new FrameTicker() { tickInterval = 5, frameCounter = 0 };

    public void Update()
    {
        if (frameTicker.WillTick() && initalized)
        {
            //Pre-initialization
            grid.blocks = new TetrisBlock[grid.width, grid.height];
            for (int i =  0; i < width;  i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid.blocks[i, j] = new TetrisBlock(0);
                }
            }
            //calculation part
            if (hasFallingBlock)
            {
                if (currentFallingTetromino.CheckCollision())
                {
                    hasFallingBlock = false;
                    currentFallingTetromino.isFrozen = true;
                }
                else
                {
                    currentFallingTetromino.MoveDown();
                    currentFallingTetromino.UpdateToGrid();
                }
            }
            else if (!hasFallingBlock)
            {
                currentFallingTetromino = new TetrisFallingTetromino(grid, Random.Range(1,7));
                hasFallingBlock = true;
                
            }
            //render part
            foreach (TetrisElement tetrisElement in FrameElements)
            {
                (int, int) tuple = FindElementIndex(FrameElements, tetrisElement);
                tetrisElement.blockType = grid.blocks[tuple.Item2,tuple.Item1].blockType;
                tetrisElement.UpdateDisplay();
            }
        }
        frameTicker.Count();
        
        
    }

    public enum MoveType
    {
        Nothing = 0,
        Down = 0,
        FastDown,
        RotateRight,
        RotateLeft
    };

    public void InitializeGame(int squareWidth, int squareHeight, int x, int y)
    {
        InitializeGrid(squareWidth, squareHeight);
       // Elements = new TetrisElement[squareHeight, squareWidth];
        width = squareWidth;
        height = squareHeight;
        Position = new Vector2(x, y);
/*
        for (int i =  0; i < squareWidth;  i++)
        {
            for (int j = 0; j < squareHeight; j++)
            {
                Elements[i,j] = 
                Elements[i, j].blockType = TetrisBlock.TetrisBlockType.Empty;
                
            }
        }
* /
        for (int i = -squareWidth/2; i <= squareWidth/2; i++)
        {
            GameObject border = Instantiate(INRuntimeGameData.Instance.GameData.m_parts.Find(o => o.GetComponent<Pig>() != null ));
            border.GetComponent<BasePart>().transform.position =
                new Vector3(transform.position.x + i, transform.position.y - 1);
            
        }

        //initialize frame element
        FrameElements = new TetrisElement[squareWidth, squareHeight];
        for (int i =  0; i < squareWidth;  i++)
        {
            for (int j = 0; j < squareHeight; j++)
            {
                
                GameObject gameObject = Instantiate(Singleton<INRuntimeGameData>.Instance.UnlistedPart.Parts[3]);
                TetrisElement tetrisElement = gameObject.GetComponent<TetrisElement>();
                tetrisElement.blockType = TetrisBlock.TetrisBlockType.Empty;
                tetrisElement.transform.position =
                    new Vector3(transform.position.x + 1 + i, transform.position.y + 1 + j);
                FrameElements[i, j] = tetrisElement;
                Contraption.Instance.Parts.Add(tetrisElement);
            }
        }
        initalized = true;
    }
    public void InitializeGrid (int squareWidth, int squareHeight)
    {
        grid = new TetrisGrid(squareWidth, squareHeight) { };
        width = squareWidth;
        height = squareHeight;
        
    }

    private void Move(MoveType moveType)
    {
    }*/
    public TetrisBoard Board;
    private FrameTicker ticker;
    private Arr2D<TetrisElement> elements;

    private readonly MoveTimer moveLeft = new MoveTimer(KeyCode.LeftArrow);
    private readonly MoveTimer moveRight = new MoveTimer(KeyCode.RightArrow);
    private readonly MoveTimer rotate = new MoveTimer(KeyCode.UpArrow);

    private bool initialized;

    private void Awake()
    {
        this.Board = new TetrisBoard();
        this.ticker = new FrameTicker();
        this.ticker.tickInterval = 30;
        this.initialized = false;
    }

    public void Init(Vector3 basePos)
    {
        this.initialized = true;
        
        //initialize frame element
        this.elements = new Arr2D<TetrisElement>(this.Board.Data.Width, this.Board.Data.Height, null);
        //so cursed lol
        GameObject prefab = Singleton<INRuntimeGameData>.Instance.UnlistedPart.Parts[3];

        for (int y = 0; y < this.elements.Height; y++)
        {
            for (int x = 0; x < this.elements.Width; x++)
            {
                GameObject obj = Instantiate(prefab);
                TetrisElement element = obj.GetComponent<TetrisElement>();
                element.boardPos = new Vector2Int(x, y);

                obj.transform.position += new Vector3(x + 1, y + 1) + basePos;
                Contraption.Instance.Parts.Add(element);

                this.elements[x, y] = element;
            }
        }
    }

    private void Update()
    {
        if (!this.initialized) return;

        bool doRedraw = false;
        bool cm = this.ticker.frameCounter % 3 == 0;
        
        //move
        this.moveLeft.Tick();
        this.moveRight.Tick();
        this.rotate.Tick();
        
        if (this.moveLeft.ShouldMove(cm))
        {
            this.Board.Falling.Move(Vector2Int.left);
            doRedraw = true;
        }
        if (this.moveRight.ShouldMove(cm))
        {
            this.Board.Falling.Move(Vector2Int.right);
            doRedraw = true;
        }
        if (this.rotate.ShouldMove(cm))
        {
            this.Board.Falling.Rotate(1);
            doRedraw = true;
        }
        if (Input.GetKey(KeyCode.DownArrow) && cm)
        {
            this.Board.Falling.Move(Vector2Int.down);
            doRedraw = true;
        }

        if (this.ticker.WillTick())
        {
            this.Board.Tick();
            doRedraw = true;
        }
        
        this.ticker.Count();
        
        if (doRedraw) Redraw();
    }

    private void Redraw()
    {
        //copy board to game
        for (int y = 0; y < this.elements.Height; y++)
        {
            for (int x = 0; x < this.elements.Width; x++)
            {
                TetrisElement e = this.elements[x, y];
                e.UpdateDisplay(this.Board.Data[x, y]);
            }
        }
            
        //copy falling tetromino
        for (int y = 0; y < this.Board.Falling.CurrentShape.Height; y++)
        {
            for (int x = 0; x < this.Board.Falling.CurrentShape.Width; x++)
            {
                if (this.Board.Falling.CurrentShape[x, y])
                {
                    int globX = x + this.Board.Falling.Position.x;
                    int globY = y + this.Board.Falling.Position.y;
                        
                    if (this.elements.OutOfBounds(globX, globY)) continue;

                    this.elements[globX, globY].UpdateDisplay(this.Board.Falling.Type.Type);
                }
            }
        }
    }
}
