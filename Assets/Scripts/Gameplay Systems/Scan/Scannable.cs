using UnityEngine;

public class Scannable : MonoBehaviour
{
    [Header("Scannable Settings")]
    [SerializeField] private ScannableSO scannableData;

    public string GetScannableID()
    {
        return scannableData.ID;
    }
    
}