using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MineSweeperManager : Singleton<MineSweeperManager>
{
    public MineSweeperElement[,] Elements;

    private int width;

    private int height;

    private int birdCount;

    private bool firstOpen = true;

    public Vector3 Position;

    public void Awake()
    {
        SetAsPersistant();
    }

    public enum SweepType
    {
        LeftClick = 0,
        ScreenTap = 0,
        RightClick = 1,
        DoubleClick = 1,
        ScreenPress = 1,
        MiddleClick = 2,
    }

    // Reference implementations from KMines https://invent.kde.org/games/kmines/-/blob/master/src/minefielditem.cpp
    //fix typo
    public void InitializeGame(int squareWidth, int squareHeight, int squareBirdCount, int x, int y)
    {
        Elements = new MineSweeperElement[squareWidth, squareHeight];
        width = squareWidth;
        height = squareHeight;
        birdCount = squareBirdCount;
        Position = new Vector3(x, y);
        // Make all empty and correctly placed
        for (int i = 0; i < squareWidth; i++)
        {
            for (int j = 0; j < squareHeight; j++)
            {
                //Create playable objects
                GameObject element = Instantiate(Singleton<INRuntimeGameData>.Instance.UnlistedPart.Parts[1]);
                Elements[i, j] = element.GetComponent<MineSweeperElement>();//fix me
                Elements[i, j].CoordX = i + x;
                Elements[i, j].CoordY = j + y;
                Elements[i, j].MatX = i;
                Elements[i, j].MatY = j;
                Elements[i, j].blockType = MineSweeperElement.MineSweeperBlockType.Empty;
                Elements[i, j].ForeGroundType = (i + j) % 2;
                Elements[i, j].Reload();
                element.transform.position = Position + new Vector3(i, j);
                Contraption.Instance.Parts.Add(Elements[i, j]);
                    
            }
        }
    }

    // Call it before update status I guess?
    public bool CheckWon()
    {
        bool unopenedElements = false;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!Elements[i, j].isOpened && Elements[i, j].blockType == MineSweeperElement.MineSweeperBlockType.Empty)
                {
                    unopenedElements = true;
                    break;
                }
            }
        }
        return !unopenedElements;
    }

    public int FlaggedElements()
    {
        int count = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Elements[i, j].isFlagged)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public void UpdateStatus(int x, int y, SweepType sweepType)
    {
        bool Bombed = Sweep(x, y, sweepType);
        if (Bombed)
        {
            Singleton<EffectManager>.Instance.CreateParticles(Singleton<INRuntimeGameData>.Instance.GameData.m_ballonParticles, Position, true);
            foreach (MineSweeperElement mineSweeperElement in Elements)
            {
                Destroy(mineSweeperElement);
            }
        }
    }

    /// <summary>
    /// Basically when clicking on an element, call this
    /// Return true if bomber
    ///
    /// -- Set to private -- Goggs
    /// </summary>
    /// <param name="x">Matrix X coord</param>
    /// <param name="y">Matrix Y coord</param>
    /// <param name="sweepType"></param>
    /// <returns></returns>
    // Basically when clicking on an element, call this
    // Return true if bomber
    private bool Sweep(int x, int y, SweepType sweepType)
    {
        Debug.Log("Sweeping");
        // Add birds only after the player reveals an element
        if (firstOpen)
        {
            firstOpen = false;
            // Add bomber birds
            for (int i = 0; i < birdCount; i++)
            {
                int randomX = Random.Range(0, width);
                int randomY = Random.Range(0, height);
                if (randomX != x && randomY != y)
                {
                    Elements[randomX, randomY].blockType = (MineSweeperElement.MineSweeperBlockType)(Random.Range(0, (int)MineSweeperElement.MineSweeperBlockType.Max) + 2);
                }
                else i--;
            }

            // Calculate digits
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int digit = 0;

                    // It's me alright? who nests functions like this...
                    bool BombAt(int x, int y)
                    {
                        if (x < 0 || x >= width || y < 0 || y >= height)
                        {
                            return false;
                        }
                        return Elements[x, y].blockType != MineSweeperElement.MineSweeperBlockType.Empty;
                    }

                    // Yeah...
                    digit += BombAt(i + 1, j + 1) ? 1 : 0;
                    digit += BombAt(i + 1, j + 0) ? 1 : 0;
                    digit += BombAt(i + 1, j - 1) ? 1 : 0;
                    digit += BombAt(i + 0, j + 1) ? 1 : 0;
                    digit += BombAt(i + 0, j - 1) ? 1 : 0;
                    digit += BombAt(i - 1, j + 1) ? 1 : 0;
                    digit += BombAt(i - 1, j + 0) ? 1 : 0;
                    digit += BombAt(i - 1, j - 1) ? 1 : 0;

                    Elements[i, j].digit = digit;
                }
            }
            RevealElements(x,y);
            
        }
        else if (sweepType == SweepType.LeftClick)
        {
            // Check if the element is a bomber bird
            if (Elements[x, y].blockType != MineSweeperElement.MineSweeperBlockType.Empty)
            {
                RevealElements(x,y,true);
                return true;
            }
            RevealElements(x,y);
            // Recursively reveal adjacent elements
           
            
        }

        // First flag, then remove flag and question your life decisions about reading this source code, then remove questioned feelings
        else if (sweepType == SweepType.RightClick)
        {
            if (Elements[x, y].isFlagged)
            {
                Elements[x, y].isFlagged = false;
                Elements[x, y].isQuestioned = true;
            }
            else if (Elements[x, y].isQuestioned)
            {
                Elements[x, y].isQuestioned = false;
            }
            else
            {
                Elements[x, y].isFlagged = true;
            }
            Elements[x, y].UpdateDisplay();
            if (CheckWon())
            {
                Singleton<EffectManager>.Instance.CreateParticles(Singleton<INRuntimeGameData>.Instance.GameData.m_ballonParticles, Position, true);
                foreach (MineSweeperElement mineSweeperElement in Elements)
                {
                    Destroy(mineSweeperElement);
                }
            }
        }
        else if (sweepType == SweepType.MiddleClick)
        {
            // ... Nothing, maybe easter egg in here?
        }
        return false;
        void RevealElements(int x, int y, bool isBomb = false)
        {
            if (isBomb)
            {
                Elements[x, y].isOpened = true;
                Elements[x, y].UpdateDisplay();
            }

            List<int[]> elementIndices = new List<int[]>();

            // Yeah... v2
            // this is hell -- Goggs
            // I know right? -- Anstro Pleuton
            if (x != 0 && y != 0) elementIndices.Add(new int[2] { x - 1, y - 1 });
            if (x != 0) elementIndices.Add(new int[2] { x - 1, y });
            if (x != 0 && y != height - 1) elementIndices.Add(new int[2] { x - 1, y + 1 });
            if (y != 0) elementIndices.Add(new int[2] { x, y - 1 });
            if (y != height - 1) elementIndices.Add(new int[2] { x, y + 1 });
            if (x != width - 1 && y != 0) elementIndices.Add(new int[2] { x + 1, y - 1 });
            if (x != width - 1) elementIndices.Add(new int[2] { x + 1, y });
            if (x != width - 1 && y != height - 1) elementIndices.Add(new int[2] { x + 1, y + 1 });

            // Recursive-ness
            foreach (var elementIndex in elementIndices)
            {
                ref MineSweeperElement element = ref Elements[elementIndex[0], elementIndex[1]];

                // Prevent infinite recursion
                if (element.isOpened || element.isFlagged || element.isQuestioned)
                {
                    continue;
                }

                // One thing that doesn't work is setting the element to be opened when the gird size is 1x1
                if (element.blockType != MineSweeperElement.MineSweeperBlockType.Bomb && element.blockType != MineSweeperElement.MineSweeperBlockType.Chuck&& element.blockType!= MineSweeperElement.MineSweeperBlockType.Red)
                {
                    element.isOpened = true;
                }

                
                // Gotta update the state of display -- Goggs
                element.UpdateDisplay();
                if (element.digit == 0)
                {
                    RevealElements(elementIndex[0], elementIndex[1]);
                }
            }
        }
    }
}
