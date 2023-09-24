using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BPManual
{
    public static Contraption Contraption => Contraption.Instance;

    public static BasePart Pig => Contraption.Instance.FindPig();
    
    public static void HELPMEEEEEE(int page)
    {
        
    } 
    public static string GetHelpPage(string page)
    {
        string header = $"Bad Piggies :: Rebooted || Console Help Page {page}\n";
        switch (page)
        {
            case "0":
                return header + "Basic command List: \n" +
                                "/help {command name} - Output usage of a command\n" +
                                "/gamerule get {gamerule name} - Get the value of a game rule\n" +
                                "/gamerule set {gamerule name} {new value} - Set the value of a game rule to a new one\n" +
                                "/shift {dx} {dy} - Shift your contraption on grid";
            case "1":
            default:
                return "Page Not Found";
        }
    }
}
