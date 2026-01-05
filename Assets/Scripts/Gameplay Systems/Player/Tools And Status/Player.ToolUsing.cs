using UnityEngine;

public partial class Player
{
    [Header("Tool Usage")]
    [SerializeField] private GameObject scanHitBox;
    [SerializeField] private float healthRegenAmount = 20f;
    [SerializeField] private GameObject baitObjectPrefab;
    [SerializeField] private Transform baitSpawnPoint;
    [SerializeField] private float detectDuration = 15f;

    [SerializeField] private GameObject scannedPopupPrefab;
    [SerializeField] private Transform popupSpawnParent; // Canvas or UI parent

    public System.Action<string> OnScanSuccess;

    private bool isAtDock = false;
    private Coroutine detectCoroutine;

    public bool SetIsAtDock(bool atDock)
    {
        isAtDock = atDock;
        return isAtDock;
    }

    private void InitScanHitBox()
    {
        if (scanHitBox != null)
        {
            scanHitBox.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Scan Hit Box is not assigned in the inspector.");
        }
    }

    private void UseToolScan()
    {
        SoundManager.Instance.PlayToolUse();
        scanHitBox.SetActive(true);
    }

    public void EndToolScan()
    {
        scanHitBox.SetActive(false);
    }

    public void SuccessfulScan(string scannableID)
    {
        OnScanSuccess?.Invoke(scannableID);

        // Spawn the popup
        if (scannedPopupPrefab != null)
        {
            Transform parent = popupSpawnParent != null ? popupSpawnParent : null;
            GameObject popupObj = Instantiate(scannedPopupPrefab, parent);

            ScannedPopup popup = popupObj.GetComponent<ScannedPopup>();
            if (popup != null)
            {
                popup.Initialize(scannableID);
            }
        }
        else
        {
            Debug.LogWarning("Scanned popup prefab is not assigned!");
        }
    }

    private void UseToolBait()
    {
        SoundManager.Instance.PlayToolUse();
        if (baitObjectPrefab == null)
        {
            Debug.LogError("Bait prefab not assigned!");
            return;
        }

        if (BaitManager.Instance == null)
        {
            Debug.LogError("BaitManager not found in scene!");
            return;
        }

        Vector3 spawnPosition = baitSpawnPoint != null ? baitSpawnPoint.position : transform.position + transform.forward * 2f;

        // Instantiate with BaitManager as parent
        Instantiate(baitObjectPrefab, spawnPosition, baitObjectPrefab.transform.rotation, BaitManager.Instance.GetTransform());
        Debug.Log($"Bait spawned at {spawnPosition}");
    }

    private void UseToolHeal()
    {
        SoundManager.Instance.PlayToolUse();
        RegenHealth(healthRegenAmount);
    }

    private void UseToolInvisible()
    {
        SoundManager.Instance.PlayToolUse();
        SetInvisibleState(true);
    }

    private void UseToolDetect()
    {
        SoundManager.Instance.PlayToolUse();
        // Stop any existing detect coroutine
        if (detectCoroutine != null)
        {
            StopCoroutine(detectCoroutine);
        }

        MiniMapCameraScript.Instance.SetCameraHeightToDetecting();
        detectCoroutine = StartCoroutine(ResetDetectAfterDelay());
    }

    private System.Collections.IEnumerator ResetDetectAfterDelay()
    {
        yield return new WaitForSeconds(detectDuration);

        if (MiniMapCameraScript.Instance != null)
        {
            MiniMapCameraScript.Instance.SetCameraHeightToNotDetecting();
        }

        detectCoroutine = null;
    }
}