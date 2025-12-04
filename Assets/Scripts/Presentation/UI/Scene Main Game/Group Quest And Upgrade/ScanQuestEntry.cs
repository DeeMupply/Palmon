using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

public class ScanQuestEntry : MonoBehaviour
{
    [Header("Scannable Settings")]
    [SerializeField] private ScannableSO scannableData;

    [Header("UI Elements")]
    [SerializeField] private Image scannableIcon;
    [SerializeField] private TextMeshProUGUI scannableNameText;
    [SerializeField] private TextMeshProUGUI scanCountText;
    private int loggedScanCount = 0;
    private int currentScanCount = 0;
    
    private void UpdateScanQuest()
    {
        
    }

    private void OnScansSubmitted()
    {
        
    }
}