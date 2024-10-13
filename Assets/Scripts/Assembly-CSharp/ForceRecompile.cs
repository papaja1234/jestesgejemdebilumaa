using UnityEditor;
using UnityEngine;

public class ForceRecompile : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Force Recompile")]
    public static void ForceRecompileScripts()
    {
        Debug.Log("Forcing recompilation of scripts.");
        EditorApplication.UnlockReloadAssemblies();
        EditorUtility.RequestScriptReload();
        AssetDatabase.Refresh();
        Debug.Log("Forced recompilation of scripts.");
    }
#endif
}
