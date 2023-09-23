using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameRules
{
    public static int PartHPStatus { get; private set; } = 0;
    public static bool ShowPropertyPanel { get; set; } = false;

    public static string SetPartHPMode(int mode)
    {
        PartHPStatus = mode;
        return mode switch
        {
            0=> "Disabled",
            1=> "Disappear only",
            _=> "Debris and No function"
        };

    }
}
