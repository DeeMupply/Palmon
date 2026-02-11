#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ForceReload 
{
    [MenuItem("Tools/Force Domain Reload")]
    public static void ReloadDomain() 
    {
        Debug.Log("🔄 Force reloading domain...");
        
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("⚠️ Cannot reload domain during play mode!");
            return;
        }
        
        EditorUtility.RequestScriptReload();
        Debug.Log("✅ Domain reload requested!");
    }
    
    [MenuItem("Tools/Force Assembly Reload")]
    public static void ReloadAssemblies()
    {
        Debug.Log("🔄 Force reloading assemblies...");
        
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("⚠️ Cannot reload assemblies during play mode!");
            return;
        }
        
        // Force assembly reload
        EditorUtility.RequestScriptReload();
        
        // Clear console to see results clearly
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod?.Invoke(null, null);
        
        Debug.Log("✅ Assembly reload requested with console clear!");
    }
}
#endif