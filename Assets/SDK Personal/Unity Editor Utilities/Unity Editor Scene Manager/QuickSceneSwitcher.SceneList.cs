#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public partial class QuickSceneSwitcher : EditorWindow
{
    // ✅ SCENE LIST UI
    void DrawSceneList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (filteredScenes.Count == 0)
        {
            if (selectedFolders.Count == 0)
            {
                EditorGUILayout.LabelField("No folders selected! Use 'Folders' to add some.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                EditorGUILayout.LabelField("No scenes found in selected folders!", EditorStyles.centeredGreyMiniLabel);
            }
        }
        else
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            string currentScenePath = activeScene.path;

            string lastFolder = "";
            foreach (var scene in filteredScenes)
            {
                // Group by folder
                if (scene.folder != lastFolder)
                {
                    if (!string.IsNullOrEmpty(lastFolder))
                        EditorGUILayout.Space();

                    EditorGUILayout.LabelField($"📁 {scene.folder}", EditorStyles.miniLabel);
                    lastFolder = scene.folder;
                }

                // Scene button with context menu
                DrawSceneButton(scene, currentScenePath);
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
    }

    // ✅ SCENE BUTTON WITH CONTEXT MENU
    void DrawSceneButton(SceneInfo scene, string currentScenePath)
    {
        bool isCurrent = scene.path == currentScenePath;
        if (isCurrent)
        {
            GUI.backgroundColor = Color.green;
        }

        string buttonText = isCurrent ? $"🎯 {scene.name}" : $"🎬 {scene.name}";

        Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(buttonText), GUI.skin.button, GUILayout.Height(25));

        Event current = Event.current;

        if (buttonRect.Contains(current.mousePosition))
        {
            if (current.type == EventType.MouseDown)
            {
                if (current.button == 1) // Right mouse button
                {
                    ShowSceneContextMenu(scene);
                    current.Use();
                    return;
                }
            }

            // Visual feedback on hover
            if (current.type == EventType.Repaint)
            {
                EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);
            }
        }

        // Draw the button normally
        if (GUI.Button(buttonRect, buttonText))
        {
            if (!isCurrent)
            {
                OpenScene(scene.path);
            }
        }

        if (isCurrent)
            GUI.backgroundColor = Color.white;
    }

    // ✅ CONTEXT MENU
    void ShowSceneContextMenu(SceneInfo scene)
    {
        GenericMenu menu = new();

        // Main actions
        if (scene.path != EditorSceneManager.GetActiveScene().path)
        {
            menu.AddItem(new GUIContent("🎬 Open Scene"), false, () => OpenScene(scene.path));
            menu.AddSeparator("");
        }

        // Asset selection (like Unity's built-in)
        menu.AddItem(new GUIContent("📁 Select Scene Asset"), false, () => SelectSceneAsset(scene.path));

        // Project window actions
        menu.AddItem(new GUIContent("🔍 Show in Project"), false, () => ShowInProject(scene.path));

        menu.AddSeparator("");

        // Copy actions
        menu.AddItem(new GUIContent("📋 Copy Scene Path"), false, () => CopyToClipboard(scene.path, "Scene path"));
        menu.AddItem(new GUIContent("📋 Copy Scene Name"), false, () => CopyToClipboard(scene.name, "Scene name"));
        menu.AddItem(new GUIContent("📋 Copy Folder Path"), false, () => CopyToClipboard(scene.folder, "Folder path"));

        menu.AddSeparator("");

        // System actions
        menu.AddItem(new GUIContent("🗂️ Show in Explorer"), false, () => ShowInExplorer(scene.path));

        // Info action
        menu.AddItem(new GUIContent("📊 Scene Info"), false, () => ShowSceneInfo(scene));

        menu.ShowAsContext();
    }

    // ✅ CONTEXT MENU ACTIONS
    void SelectSceneAsset(string scenePath)
    {
        UnityEngine.Object sceneAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scenePath);
        if (sceneAsset != null)
        {
            Selection.activeObject = sceneAsset;
            EditorGUIUtility.PingObject(sceneAsset);
            Debug.Log($"📁 Selected scene asset: {Path.GetFileNameWithoutExtension(scenePath)}");
        }
        else
        {
            Debug.LogError($"❌ Could not find scene asset: {scenePath}");
        }
    }

    void ShowInProject(string scenePath)
    {
        UnityEngine.Object sceneAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scenePath);
        if (sceneAsset != null)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = sceneAsset;
            EditorGUIUtility.PingObject(sceneAsset);
            Debug.Log($"🔍 Showed in project: {Path.GetFileNameWithoutExtension(scenePath)}");
        }
    }

    void ShowInExplorer(string scenePath)
    {
        string fullPath = Path.GetFullPath(scenePath);
        if (File.Exists(fullPath))
        {
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
#elif UNITY_EDITOR_OSX
            System.Diagnostics.Process.Start("open", $"-R \"{fullPath}\"");
#elif UNITY_EDITOR_LINUX
            System.Diagnostics.Process.Start("xdg-open", $"\"{Path.GetDirectoryName(fullPath)}\"");
#endif
            Debug.Log($"🗂️ Showed in explorer: {fullPath}");
        }
        else
        {
            Debug.LogError($"❌ File not found: {fullPath}");
        }
    }

    void ShowSceneInfo(SceneInfo scene)
    {
        string fullPath = Path.GetFullPath(scene.path);
        FileInfo fileInfo = new(fullPath);

        string info = $"Scene Information:\n" +
                      $"Name: {scene.name}\n" +
                      $"Path: {scene.path}\n" +
                      $"Folder: {scene.folder}\n" +
                      $"File Size: {FormatBytes(fileInfo.Length)}\n" +
                      $"Last Modified: {fileInfo.LastWriteTime}\n" +
                      $"Created: {fileInfo.CreationTime}";

        EditorUtility.DisplayDialog("Scene Info", info, "OK");
        Debug.Log($"📊 Scene Info:\n{info}");
    }
}

#endif