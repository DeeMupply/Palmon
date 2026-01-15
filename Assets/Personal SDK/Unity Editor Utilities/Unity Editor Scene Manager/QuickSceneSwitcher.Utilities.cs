#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public partial class QuickSceneSwitcher : EditorWindow
{
    // ✅ PROJECT-SCOPED EDITORPREFS KEYS (per-user, per-project)
    string GetProjectPrefsKey(string baseKey)
    {
        // dataPath is unique per project on the machine
        int projectHash = Application.dataPath.GetHashCode();
        return $"{baseKey}_{projectHash}";
    }

    // ✅ UTILITY METHODS
    void CopyToClipboard(string text, string description)
    {
        EditorGUIUtility.systemCopyBuffer = text;
        Debug.Log($"📋 Copied {description}: {text}");
    }

    // ✅ FIXED: Correct byte formatting
    string FormatBytes(long bytes)
    {
        if (bytes == 0) return "0 bytes";

        const int scale = 1024;
        string[] sizes = { "bytes", "KB", "MB", "GB", "TB" };

        int order = 0;
        double size = bytes;

        while (size >= scale && order < sizes.Length - 1)
        {
            order++;
            size = size / scale;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}

#endif