using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperManager : Singleton<MinesweeperManager>
{
    public MineSweeperElement[,] Elements;

    public void InitializeGame(int squareWidth, int birdCount, int x, int y)
    {
        Elements = new MineSweeperElement[squareWidth, squareWidth];
        int index = 0;
        foreach (var mineSweeperElement in Elements)
        {
            mineSweeperElement.ForeGroundType = index % 2;
            mineSweeperElement.CoordX = x + (index % squareWidth);
            mineSweeperElement.CoordY = y + (index % squareWidth);
            
            index++;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
