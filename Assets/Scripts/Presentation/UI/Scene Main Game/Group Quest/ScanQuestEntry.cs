using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScanQuestEntry : MonoBehaviour
{
    [Header("Scannable Settings")]
    [SerializeField] private ScannableSO scannableData;

    [Header("UI Elements")]
    [SerializeField] private Image scannableIcon;
    [SerializeField] private TextMeshProUGUI scannableNameText;
    [SerializeField] private TextMeshProUGUI adnPerScanText;
    [SerializeField] private TextMeshProUGUI scanCountText;
    [SerializeField] private TextMeshProUGUI scanRequiredText;

    [SerializeField] private Slider scanLoggedProgressSlider;
    [SerializeField] private Slider scanCurrentProgressSlider;
    private int loggedScanCount = 0;
    private int currentScanCount = 0;

    public void SetEventHandlers()
    {
        Player.Instance.OnScansSubmitted += OnScansSubmitted;
    }

    private void OnDisable()
    {
        Player.Instance.OnScansSubmitted -= OnScansSubmitted;
    }

    public void Initialize(ScannableSO scannable)
    {
        scannableData = scannable;
        scannableIcon.sprite = scannableData.ScannableIcon;
        scannableNameText.text = scannableData.ScannableName;
        adnPerScanText.text = $"{scannableData.AdnPerScan}";
        scanRequiredText.text = $"/{scannableData.ScansRequired}";
        scanLoggedProgressSlider.maxValue = scannableData.ScansRequired;
        scanCurrentProgressSlider.maxValue = scannableData.ScansRequired;
        loggedScanCount = 0;
        currentScanCount = 0;
        UpdateScanQuest();
    }

    private void UpdateScanQuest()
    {
        scanCountText.text = $"{loggedScanCount + currentScanCount}";
        scanLoggedProgressSlider.value = loggedScanCount;
        scanCurrentProgressSlider.value = currentScanCount;
    }

    public void OnSuccessfulScan()
    {
        currentScanCount++;
        UpdateScanQuest();
    }

    private void OnScansSubmitted()
    {
        Player.Instance.AddADN(currentScanCount * scannableData.AdnPerScan);
        loggedScanCount += currentScanCount;
        currentScanCount = 0;
        UpdateScanQuest();
    }

    // Add this method to your existing ScanQuestEntry class:
    public string GetScannableID()
    {
        return scannableData != null ? scannableData.ID : "";
    }

    public bool IsQuestCompleted()
    {
        if (scannableData == null)
            return false;

        return loggedScanCount >= scannableData.ScansRequired;
    }

    public int GetLoggedScanCount()
    {
        return loggedScanCount;
    }

    public int GetTotalScanCount()
    {
        return loggedScanCount + currentScanCount;
    }
}