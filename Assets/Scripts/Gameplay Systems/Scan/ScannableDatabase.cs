using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ScannableDatabase", menuName = "Gameplay/Scannable Database")]
public class ScannableDatabase : ScriptableObject
{
    [Header("Database")]
    [SerializeField] private List<ScannableSO> allScannables = new List<ScannableSO>();
    
    // Dictionary for fast lookup (not serialized)
    private Dictionary<string, ScannableSO> scannableDictionary = new Dictionary<string, ScannableSO>();
    
    #region Public Access Methods
    
    public ScannableSO GetScannableByID(string id)
    {
        if (scannableDictionary.Count == 0)
        {
            GenerateDictionary();
        }
        
        scannableDictionary.TryGetValue(id, out ScannableSO scannable);
        return scannable;
    }
    
    public List<ScannableSO> GetAllScannables()
    {
        return new List<ScannableSO>(allScannables);
    }
    
    public bool ContainsScannable(string id)
    {
        if (scannableDictionary.Count == 0)
        {
            GenerateDictionary();
        }
        
        return scannableDictionary.ContainsKey(id);
    }
    
    public int GetScannableCount()
    {
        return allScannables.Count;
    }
    
    #endregion
    
    #region Dictionary Generation
    
    private void GenerateDictionary()
    {
        scannableDictionary.Clear();
        
        foreach (var scannable in allScannables)
        {
            if (scannable != null)
            {
                if (scannableDictionary.ContainsKey(scannable.ID))
                {
                    Debug.LogWarning($"Duplicate scannable ID found: {scannable.ID}. Skipping {scannable.name}");
                    continue;
                }
                scannableDictionary.Add(scannable.ID, scannable);
            }
        }
        
        Debug.Log($"Generated scannable dictionary with {scannableDictionary.Count} entries");
    }
    
    #endregion
    
    #region Editor Only - Auto Population
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        
        PopulateScannablesList();
        GenerateDictionary();
    }
    
    private void PopulateScannablesList()
    {
        // Find all ScannableSO assets in the project
        string[] guids = AssetDatabase.FindAssets("t:ScannableSO");
        allScannables.Clear();
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScannableSO scannable = AssetDatabase.LoadAssetAtPath<ScannableSO>(path);
            
            if (scannable != null)
            {
                allScannables.Add(scannable);
            }
        }
        
        // Sort by name for better organization
        allScannables = allScannables.OrderBy(s => s.ScannableName).ToList();
        
        Debug.Log($"Found and populated {allScannables.Count} scannables in database");
        
        // Mark dirty to save changes
        EditorUtility.SetDirty(this);
    }
    
    [ContextMenu("Force Refresh Database")]
    private void ForceRefresh()
    {
        PopulateScannablesList();
        GenerateDictionary();
    }
    
    [ContextMenu("Log All Scannables")]
    private void LogAllScannables()
    {
        Debug.Log($"=== Scannable Database Contents ({allScannables.Count} items) ===");
        foreach (var scannable in allScannables)
        {
            if (scannable != null)
            {
                Debug.Log($"- {scannable.ScannableName} (ID: {scannable.ID}) - {scannable.ScansRequired} scans, {scannable.AdnPerScan} ADN per scan");
            }
        }
    }
#endif
    
    #endregion
    
    #region Initialization
    
    private void OnEnable()
    {
        // Generate dictionary when the ScriptableObject is loaded
        if (allScannables.Count > 0 && scannableDictionary.Count == 0)
        {
            GenerateDictionary();
        }
    }
    
    #endregion
}