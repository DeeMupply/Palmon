#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Linq;

public partial class QuickSceneSwitcher : EditorWindow
{
    // ✅ EDITORPREFS KEYS
    private const string PREFS_FOLDERS_KEY = "QuickSceneSwitcher_Folders";
    private const string PREFS_SHOW_SETTINGS_KEY = "QuickSceneSwitcher_ShowSettings";
    private const string PREFS_FOLDER_HEIGHT_KEY = "QuickSceneSwitcher_FolderHeight";
    private const string PREFS_FOLDER_STATES_KEY = "QuickSceneSwitcher_FolderStates"; // ✅ NEW

    // ✅ SETTINGS PERSISTENCE
    void SaveSettings()
    {
        string foldersJson = string.Join(";", selectedFolders);
        EditorPrefs.SetString(PREFS_FOLDERS_KEY, foldersJson);
        EditorPrefs.SetBool(PREFS_SHOW_SETTINGS_KEY, showFolderSettings);
        EditorPrefs.SetFloat(PREFS_FOLDER_HEIGHT_KEY, folderAreaHeight);
        
        // ✅ NEW: Save folder enabled states
        var statesJson = string.Join(";", folderEnabledStates.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
        EditorPrefs.SetString(PREFS_FOLDER_STATES_KEY, statesJson);
        
        Debug.Log($"💾 Saved settings: {selectedFolders.Count} folders, height: {folderAreaHeight}");
    }

    void LoadSettings()
    {
        string foldersJson = EditorPrefs.GetString(PREFS_FOLDERS_KEY, "");
        if (!string.IsNullOrEmpty(foldersJson))
        {
            selectedFolders = foldersJson.Split(';').Where(f => !string.IsNullOrEmpty(f)).ToList();
            Debug.Log($"📂 Loaded settings: {selectedFolders.Count} folders");
        }

        showFolderSettings = EditorPrefs.GetBool(PREFS_SHOW_SETTINGS_KEY, false);
        folderAreaHeight = EditorPrefs.GetFloat(PREFS_FOLDER_HEIGHT_KEY, 200f);
        
        // ✅ NEW: Load folder enabled states
        string statesJson = EditorPrefs.GetString(PREFS_FOLDER_STATES_KEY, "");
        folderEnabledStates.Clear();
        if (!string.IsNullOrEmpty(statesJson))
        {
            foreach (string stateEntry in statesJson.Split(';'))
            {
                if (stateEntry.Contains(':'))
                {
                    string[] parts = stateEntry.Split(':');
                    if (parts.Length == 2 && bool.TryParse(parts[1], out bool enabled))
                    {
                        folderEnabledStates[parts[0]] = enabled;
                    }
                }
            }
        }
    }
}

#endif