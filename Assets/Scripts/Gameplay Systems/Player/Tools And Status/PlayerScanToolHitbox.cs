using UnityEngine;

public class PlayerScanToolHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"PlayerScanToolHitbox detected collision with {other.gameObject.name}");
        if (other.gameObject.TryGetComponent(out Scannable scannable))
        {
            Debug.Log($"Scannable detected with ID: {scannable.GetScannableID()}");
            Player.Instance.SuccessfulScan(scannable.GetScannableID());
        }
    }
}