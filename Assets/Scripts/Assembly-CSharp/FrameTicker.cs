using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameTicker
{
    public int tickInterval;
    public int frameCounter = 0;

    public void Count()
    {
        frameCounter++;
    }

    public bool WillTick()
    {
        return frameCounter % tickInterval == 0;
    }
}
