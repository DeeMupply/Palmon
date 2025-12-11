using UnityEngine;

public partial class Player
{
    [Header("Tool Usage")]
    [SerializeField] private GameObject scanHitBox;
    [SerializeField] private float healthRegenAmount = 20f;
    
    public System.Action<string> OnScanSuccess;
    private void UseToolScan()
    {
        scanHitBox.SetActive(true);
    }

    public void EndToolScan()
    {
        scanHitBox.SetActive(false);
    }

    public void SuccessfulScan(string scannableID)
    {
        OnScanSuccess?.Invoke(scannableID);
    }
    
    private void UseToolBait()
    {
        // Implement bait tool usage logic here
    }

    private void UseToolHeal()
    {
        RegenHealth(healthRegenAmount);
    }

    private void UseToolInvisible()
    {
        // Implement invisible tool usage logic here
    }

    private void UseToolDetect()
    {
        // Implement detect tool usage logic here
    }
}