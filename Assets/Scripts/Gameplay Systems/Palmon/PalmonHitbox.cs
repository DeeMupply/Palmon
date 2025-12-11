using UnityEngine;

public class PalmonHitbox : MonoBehaviour
{
    [SerializeField] private Palmon palmon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.TakeDamage(palmon.GetAttackDamage());
        }
    }
}