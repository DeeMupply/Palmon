using UnityEngine;
using UnityEditor;
using System.IO;

public class IconRenderer : MonoBehaviour
{
    [MenuItem("Tools/Render Icon")]
    static void RenderIcon()
    {
        Camera cam = Camera.main;
        RenderTexture rt = cam.targetTexture;

        // Force the camera to render a fresh frame
        cam.Render();   // VERY IMPORTANT FOR URP

        // Set RT active
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        // Create texture buffer
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        // Restore
        RenderTexture.active = previous;

        // Save file
        string path = EditorUtility.SaveFilePanel("Save Icon", "Assets/", "ItemIcon.png", "png");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.Refresh();
        }

        Debug.Log("Icon saved to " + path);
    }
}
