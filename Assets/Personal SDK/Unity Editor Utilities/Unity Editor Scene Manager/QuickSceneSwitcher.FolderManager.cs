#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public partial class QuickSceneSwitcher : EditorWindow
{
    // ✅ FOLDER DISCOVERY
    void AutoDiscoverSceneFolders()
    {
        Debug.Log("🔍 Auto-discovering scene folders...");

        discoveredFolders.Clear();

        // Find all .unity files in the project
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        HashSet<string> foldersWithScenes = new();

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            string folderPath = Path.GetDirectoryName(scenePath).Replace("\\", "/");
            foldersWithScenes.Add(folderPath);
        }

        // ✅ FIXED: Proper alphabetical sorting A-Z
        discoveredFolders = foldersWithScenes.OrderBy(f => f).ToList();

        Debug.Log($"🔍 Discovered {discoveredFolders.Count} folders with scenes:");
        foreach (string folder in discoveredFolders)
        {
            Debug.Log($"  📁 {folder}");
        }

        // If no folders are selected yet, use all discovered folders
        if (selectedFolders.Count == 0)
        {
            selectedFolders = new List<string>(discoveredFolders);
            Debug.Log("✅ Auto-selected all discovered folders");
        }

        // ✅ NEW: Initialize folder enabled states for new folders
        foreach (string folder in selectedFolders)
        {
            if (!folderEnabledStates.ContainsKey(folder))
            {
                folderEnabledStates[folder] = true; // Default to enabled
            }
        }
    }

    // ✅ NEW: Get sorted folders list (enabled first, then disabled, both alphabetical)
    List<string> GetSortedFoldersList()
    {
        var enabledFolders = selectedFolders
            .Where(f => folderEnabledStates.GetValueOrDefault(f, true))
            .OrderBy(f => f)
            .ToList();

        var disabledFolders = selectedFolders
            .Where(f => !folderEnabledStates.GetValueOrDefault(f, true))
            .OrderBy(f => f)
            .ToList();

        var result = new List<string>();
        result.AddRange(enabledFolders);
        result.AddRange(disabledFolders);
        return result;
    }

    // ✅ UPDATED: Resizable folder management with checkboxes
    void DrawResizableFolderManagement()
    {
        // Calculate available height more carefully
        float windowHeight = position.height;
        float usedHeight = 140f; // Header + controls + footer (more conservative estimate)
        float availableHeight = windowHeight - usedHeight;

        // Clamp folder area height
        folderAreaHeight = Mathf.Clamp(folderAreaHeight, MIN_FOLDER_HEIGHT, Mathf.Min(MAX_FOLDER_HEIGHT, availableHeight * 0.6f));

        // ✅ FOLDER MANAGEMENT AREA
        EditorGUILayout.BeginVertical("box", GUILayout.Height(folderAreaHeight));

        EditorGUILayout.LabelField("📁 Folder Management", EditorStyles.boldLabel);

        // Action buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("➕ Add Folder"))
        {
            AddCustomFolder();
        }
        if (GUILayout.Button("🔄 Reset to Discovered"))
        {
            ResetToDiscoveredFolders();
        }
        if (GUILayout.Button("❌ Clear All"))
        {
            ClearAllFolders();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // ✅ UPDATED: Folder list with checkboxes and proper sorting
        var enabledCount = selectedFolders.Count(f => folderEnabledStates.GetValueOrDefault(f, true));
        EditorGUILayout.LabelField($"Selected Folders ({enabledCount}/{selectedFolders.Count} enabled):", EditorStyles.miniLabel);

        float folderListHeight = folderAreaHeight - 90f; // Account for buttons and labels
        folderScrollPosition = EditorGUILayout.BeginScrollView(folderScrollPosition, GUILayout.Height(folderListHeight));

        // ✅ NEW: Use sorted folders list (enabled first, then disabled)
        var sortedFolders = GetSortedFoldersList();
        
        for (int i = 0; i < sortedFolders.Count; i++)
        {
            string folder = sortedFolders[i];
            EditorGUILayout.BeginHorizontal();

            // ✅ NEW: Checkbox for enable/disable
            bool wasEnabled = folderEnabledStates.GetValueOrDefault(folder, true);
            bool isEnabled = EditorGUILayout.Toggle(wasEnabled, GUILayout.Width(20));
            
            if (isEnabled != wasEnabled)
            {
                folderEnabledStates[folder] = isEnabled;
                RefreshSceneList();
                SaveSettings();
            }

            // Folder status indicator
            bool exists = AssetDatabase.IsValidFolder(folder);
            bool hasScenes = discoveredFolders.Contains(folder);

            string statusIcon = "📁";
            if (!exists) statusIcon = "❌";
            else if (!hasScenes) statusIcon = "📂";

            // ✅ VISUAL: Different style for disabled folders
            GUIStyle folderStyle = isEnabled ? EditorStyles.label : EditorStyles.miniLabel;
            Color originalColor = GUI.color;
            if (!isEnabled)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f); // Gray out disabled folders
            }

            // Folder path
            EditorGUILayout.LabelField($"{statusIcon} {folder}", folderStyle);

            GUI.color = originalColor;

            // ✅ NEW: Remove button (find original index in selectedFolders)
            if (GUILayout.Button("🗑️", GUILayout.Width(30)))
            {
                selectedFolders.Remove(folder);
                folderEnabledStates.Remove(folder);
                RefreshSceneList();
                SaveSettings();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // ✅ UPDATED: Legend with checkbox info
        EditorGUILayout.LabelField("☑️=Enabled | ☐=Disabled | 📁=Valid+Scenes | 📂=Valid, no scenes | ❌=Missing", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();

        // ✅ RESIZABLE SPLITTER
        DrawResizableSplitter();

        // ✅ SCENE AREA
        EditorGUILayout.BeginVertical();

        // Search
        EditorGUI.BeginChangeCheck();
        searchFilter = EditorGUILayout.TextField("🔍 Search:", searchFilter);
        if (EditorGUI.EndChangeCheck())
        {
            FilterScenes();
        }
        EditorGUILayout.Space();

        // Current scene info
        DrawCurrentSceneInfo();

        // Scene list
        DrawSceneList();

        EditorGUILayout.EndVertical();
    }

    // ✅ OFFICIAL: Clean version of the working splitter method
    void DrawResizableSplitter()
    {
        // Create a rect for the splitter
        Rect splitterRect = EditorGUILayout.GetControlRect(false, SPLITTER_HEIGHT);

        // Handle mouse events for resizing
        Event e = Event.current;

        // ✅ CHECK: Is mouse hovering over splitter?
        bool isHovering = splitterRect.Contains(e.mousePosition);

        // ✅ DETECT HOVER CHANGES: Force repaint when hover state changes
        if (isHovering != wasHoveringLastFrame)
        {
            wasHoveringLastFrame = isHovering;
            Repaint();
        }

        // ✅ ALWAYS: Set cursor when hovering
        if (isHovering)
        {
            EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeVertical);
        }

        // ✅ VISUAL: Different colors for different states
        Color splitterColor;
        if (isResizing)
        {
            splitterColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Brightest when dragging
        }
        else if (isHovering)
        {
            splitterColor = new Color(0.6f, 0.6f, 0.6f, 1f); // Medium when hovering
        }
        else
        {
            splitterColor = new Color(0.4f, 0.4f, 0.4f, 1f); // Dark when normal
        }

        EditorGUI.DrawRect(splitterRect, splitterColor);

        // ✅ HANDLE EVENTS
        if (e.type == EventType.MouseDown && isHovering && e.button == 0)
        {
            isResizing = true;
            e.Use();
        }

        if (isResizing)
        {
            if (e.type == EventType.MouseDrag)
            {
                folderAreaHeight += e.delta.y;
                folderAreaHeight = Mathf.Clamp(folderAreaHeight, MIN_FOLDER_HEIGHT, MAX_FOLDER_HEIGHT);
                Repaint();
                e.Use();
            }
            else if (e.type == EventType.MouseUp)
            {
                isResizing = false;
                SaveSettings();
                e.Use();
            }
        }

        Repaint();
    }

    // ✅ FOLDER ACTIONS
    void AddCustomFolder()
    {
        string selectedPath = EditorUtility.OpenFolderPanel("Select Scene Folder", "Assets", "");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            // Convert absolute path to relative
            if (selectedPath.StartsWith(Application.dataPath))
            {
                string relativePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                relativePath = relativePath.Replace("\\", "/");

                if (!selectedFolders.Contains(relativePath))
                {
                    selectedFolders.Add(relativePath);
                    // ✅ FIXED: Keep alphabetical sorting when adding
                    selectedFolders = selectedFolders.OrderBy(f => f).ToList();
                    folderEnabledStates[relativePath] = true; // ✅ NEW: Default to enabled
                    RefreshSceneList();
                    SaveSettings();
                    Debug.Log($"➕ Added folder: {relativePath}");
                }
                else
                {
                    Debug.Log($"⚠️ Folder already selected: {relativePath}");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Selected folder must be within the project Assets folder!");
            }
        }
    }

    void ResetToDiscoveredFolders()
    {
        selectedFolders = new List<string>(discoveredFolders);
        // ✅ NEW: Reset all folder states to enabled
        folderEnabledStates.Clear();
        foreach (string folder in selectedFolders)
        {
            folderEnabledStates[folder] = true;
        }
        RefreshSceneList();
        SaveSettings();
        Debug.Log("🔄 Reset to discovered folders");
    }

    void ClearAllFolders()
    {
        selectedFolders.Clear();
        folderEnabledStates.Clear(); // ✅ NEW: Clear enabled states too
        RefreshSceneList();
        SaveSettings();
        Debug.Log("❌ Cleared all folders");
    }

    // ✅ UPDATED: Scene list refresh (only use enabled folders)
    void RefreshSceneList()
    {
        allScenes.Clear();

        // ✅ NEW: Only use enabled folders for scene loading
        var enabledFolders = selectedFolders
            .Where(f => folderEnabledStates.GetValueOrDefault(f, true))
            .Where(AssetDatabase.IsValidFolder)
            .ToArray();

        if (enabledFolders.Length == 0)
        {
            Debug.LogWarning("⚠️ No enabled folders found! Enable some folders to see scenes.");
            FilterScenes();
            return;
        }

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", enabledFolders);
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            allScenes.Add(new SceneInfo(scenePath));
        }

        // ✅ FIXED: Proper alphabetical sorting A-Z (folder first, then name)
        allScenes = allScenes.OrderBy(s => s.folder).ThenBy(s => s.name).ToList();

        FilterScenes();
        Debug.Log($"🔄 Found {allScenes.Count} scenes in {enabledFolders.Length} enabled folders: {string.Join(", ", enabledFolders)}");
    }
}

#endif