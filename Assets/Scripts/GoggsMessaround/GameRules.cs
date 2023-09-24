using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Entities.UniversalDelegates;
using Unity.Properties;
using UnityEngine;

public static class GameRules
{
    public static int PartHPStatus { get; private set; } = 2;
    public static bool ShowPropertyPanel { get; set; } = false;
    public static float TerrainScale { get; set; } = 1f;
    
    public static string PlayerName { get; set; } = "Player";
    private static string SetPartHPMode(int mode)
    {
        PartHPStatus = mode;
        return mode switch
        {
            0=> "Disabled",
            1=> "Disappear only",
            _=> "Debris and No function"
        };

    }

    public static void SetGameRule(string name, string value)
    {
        switch (name)
        {
            case "parthpstatus" or "parthpmode":
                SetPartHPMode(Convert.ToInt32(value));
                break;
            case "showpropertypanel":
                ShowPropertyPanel = value.ToLower() == "true";
                break;
            case "terrrainscale":
                TerrainScale = Convert.ToSingle(value);
                break;
            case "playername":
                PlayerName = value;
                break;
            default:
                Console.WriteLine(INLocalization.Instance.GetText("Gamerule_NotFound"));
                break;
        }
    }
    public static string GetGameRuleString(string name)
    {
        return name switch
        {
            "parthpstatus" => PartHPStatus.ToString(),
            "showpropertypanel" => ShowPropertyPanel.ToString(),
            "terrainscale" => TerrainScale.ToString(CultureInfo.InvariantCulture),
            "playername" => PlayerName,
            _ => INLocalization.Instance.GetText("Gamerule_NotFound")
        };
    }
}
