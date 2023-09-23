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
                return header + "";
            case "1":
            default:
                return "Page Not Found";
        }
    }
}
