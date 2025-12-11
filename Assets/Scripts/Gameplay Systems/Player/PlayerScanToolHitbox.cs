using UnityEngine;

public class PlayerScanToolHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Scannable scannable))
        {
            Player.Instance.SuccessfulScan(scannable.GetScannableID());
        }
    }
}