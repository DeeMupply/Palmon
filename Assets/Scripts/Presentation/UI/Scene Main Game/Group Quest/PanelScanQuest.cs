using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelScanQuest : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI totalAdnText;
    [SerializeField] private GameObject scanQuestEntryPrefab;
    [SerializeField] private ScannableDatabase scannableDatabase;
    [SerializeField] private Transform scanQuestEntryContainer;

    // Keep track of created entries
    private List<ScanQuestEntry> scanQuestEntries = new List<ScanQuestEntry>();

    private void Start()
    {
        InitializeScanQuestEntries();
    }

    private void OnEnable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnAdnChanged += UpdateTotalAdn;
        }
    }

    private void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnAdnChanged -= UpdateTotalAdn;
        }
    }

    private void InitializeScanQuestEntries()
    {
        if (scannableDatabase == null)
        {
            Debug.LogError("ScannableDatabase is not assigned to PanelScanQuest!");
            return;
        }

        if (scanQuestEntryPrefab == null)
        {
            Debug.LogError("ScanQuestEntryPrefab is not assigned to PanelScanQuest!");
            return;
        }

        if (scanQuestEntryContainer == null)
        {
            Debug.LogError("ScanQuestEntryContainer is not assigned to PanelScanQuest!");
            return;
        }

        // Clear any existing entries
        ClearExistingEntries();

        // Get all scannables from database
        List<ScannableSO> allScannables = scannableDatabase.GetAllScannables();

        // Create quest entry for each scannable
        foreach (ScannableSO scannable in allScannables)
        {
            if (scannable != null)
            {
                CreateScanQuestEntry(scannable);
            }
        }

        Debug.Log($"Initialized {scanQuestEntries.Count} scan quest entries");
    }

    private void CreateScanQuestEntry(ScannableSO scannable)
    {
        // Instantiate the prefab
        GameObject entryObject = Instantiate(scanQuestEntryPrefab, scanQuestEntryContainer);

        // Get the ScanQuestEntry component
        ScanQuestEntry scanQuestEntry = entryObject.GetComponent<ScanQuestEntry>();

        if (scanQuestEntry != null)
        {
            // Initialize with scannable data
            scanQuestEntry.Initialize(scannable);
            
            // Set up event handlers
            scanQuestEntry.SetEventHandlers();

            // Add to our list for tracking
            scanQuestEntries.Add(scanQuestEntry);
        }
        else
        {
            Debug.LogError($"ScanQuestEntry component not found on instantiated prefab for {scannable.ScannableName}");
            Destroy(entryObject); // Clean up the failed instantiation
        }
    }

    private void ClearExistingEntries()
    {
        // Clear the tracking list
        scanQuestEntries.Clear();

        // Destroy all existing child entries
        for (int i = scanQuestEntryContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = scanQuestEntryContainer.GetChild(i);
            if (child != null)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public void UpdateTotalAdn(int totalAdn)
    {
        totalAdnText.text = totalAdn.ToString();
    }

    // Public method to refresh entries if database changes
    public void RefreshScanQuestEntries()
    {
        InitializeScanQuestEntries();
    }

    // Get a specific scan quest entry by scannable ID
    public ScanQuestEntry GetScanQuestEntry(string scannableID)
    {
        foreach (ScanQuestEntry entry in scanQuestEntries)
        {
            if (entry != null && entry.GetScannableID() == scannableID)
            {
                return entry;
            }
        }
        return null;
    }

    // Get all scan quest entries
    public List<ScanQuestEntry> GetAllScanQuestEntries()
    {
        return new List<ScanQuestEntry>(scanQuestEntries);
    }

    [ContextMenu("Force Refresh Entries")]
    private void ForceRefreshEntries()
    {
        RefreshScanQuestEntries();
    }
}