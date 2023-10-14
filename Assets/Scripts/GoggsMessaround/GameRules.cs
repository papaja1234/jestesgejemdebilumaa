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
    public static bool PlaySound { get; set; } = false;
    public static string PlayerName { get; set; } = "Player";

    public static bool ShowCommandLog { get; set; } = true;

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
                SetPartHPMode(Int32.Parse(value));
                break;
            case "showpropertypanel":
                ShowPropertyPanel = Boolean.Parse(value);
                break;
            case "terrainscale":
                TerrainScale = Single.Parse(value);
                break;
            case "playername":
                PlayerName = value;
                break;
            case "showcommandlog":
                ShowCommandLog = Boolean.Parse(value);
                break;
            case "playsound":
                PlaySound = Boolean.Parse(value);
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
            "showcommandlog" => ShowCommandLog.ToString(),
            "playsound" => PlaySound.ToString(),
            _ => INLocalization.Instance.GetText("Gamerule_NotFound")
        };
    }
}
