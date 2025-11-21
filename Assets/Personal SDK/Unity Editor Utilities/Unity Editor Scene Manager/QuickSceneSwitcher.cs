#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public partial class QuickSceneSwitcher : EditorWindow
{
    // ✅ CORE UI STATE
    private Vector2 scrollPosition;
    private Vector2 folderScrollPosition;
    private string searchFilter = "";

    // ✅ SCENE DATA
    private List<SceneInfo> allScenes = new();
    private List<SceneInfo> filteredScenes = new();

    // ✅ FOLDER MANAGEMENT
    private List<string> selectedFolders = new();
    private List<string> discoveredFolders = new();
    private Dictionary<string, bool> folderEnabledStates = new(); // ✅ NEW
    private bool showFolderSettings = false;
    private bool wasHoveringLastFrame = false;

    // ✅ NEW: Resizable splitter variables
    private float folderAreaHeight = 200f;
    private bool isResizing = false;
    private const float MIN_FOLDER_HEIGHT = 100f;
    private const float MAX_FOLDER_HEIGHT = 400f;
    private const float SPLITTER_HEIGHT = 6f;

    [System.Serializable]
    public class SceneInfo
    {
        public string name;
        public string path;
        public string folder;

        public SceneInfo(string scenePath)
        {
            path = scenePath;
            name = Path.GetFileNameWithoutExtension(scenePath);
            folder = Path.GetDirectoryName(scenePath).Replace("\\", "/");
        }
    }

    [MenuItem("Tools/Quick Scene Switcher")]
    public static void ShowWindow()
    {
        QuickSceneSwitcher window = GetWindow<QuickSceneSwitcher>("Scene Switcher");
        window.minSize = new Vector2(350, 450);
        window.LoadSettings();
        window.AutoDiscoverSceneFolders();
        window.RefreshSceneList();
    }

    void OnEnable()
    {
        LoadSettings();
        AutoDiscoverSceneFolders();
        RefreshSceneList();
    }

    void OnDisable()
    {
        SaveSettings();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        // Header
        EditorGUILayout.LabelField("Quick Scene Switcher", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Controls
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔄 Refresh", GUILayout.Width(80)))
        {
            RefreshSceneList();
        }

        if (GUILayout.Button("🔍 Rescan", GUILayout.Width(80)))
        {
            AutoDiscoverSceneFolders();
            RefreshSceneList();
        }

        showFolderSettings = EditorGUILayout.Toggle("📁 Folders", showFolderSettings, GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();

        // ✅ UPDATED: Resizable folder management or regular scene view
        if (showFolderSettings)
        {
            DrawResizableFolderManagement();
        }
        else
        {
            // Regular scene view without folder management
            DrawRegularSceneView();
        }

        // Footer
        EditorGUILayout.LabelField($"Total scenes: {allScenes.Count} | Filtered: {filteredScenes.Count} | Folders: {selectedFolders.Count}", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }

    // ✅ NEW: Regular scene view (when folder management is hidden)
    void DrawRegularSceneView()
    {
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
    }

    void DrawCurrentSceneInfo()
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        string currentScenePath = activeScene.path;
        string currentSceneName = activeScene.name;

        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorGUILayout.LabelField($"Current: {currentSceneName}", EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Path: {currentScenePath}", EditorStyles.miniLabel);
        }
        else
        {
            EditorGUILayout.LabelField("No scene loaded", EditorStyles.helpBox);
        }
        EditorGUILayout.Space();
    }

    void FilterScenes()
    {
        if (string.IsNullOrEmpty(searchFilter))
        {
            filteredScenes = new List<SceneInfo>(allScenes);
        }
        else
        {
            filteredScenes = allScenes.Where(s =>
                s.name.ToLower().Contains(searchFilter.ToLower())
            ).ToList();
        }
    }

    void OpenScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
            Debug.Log($"🎬 Opened scene: {Path.GetFileNameWithoutExtension(path)}");
        }
    }
}

#endif