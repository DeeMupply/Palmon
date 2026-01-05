using UnityEngine;

[CreateAssetMenu(fileName = "New Palmon", menuName = "Gameplay Systems/Palmon/Palmon")]
public class PalmonSO : ScriptableObject
{
    [Header("Basic Info")]
    public string palmonName = "New Palmon";
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    public float rotationSpeed = 2f;
    public float rotationThresholdAngle = 10f;
    
    [Header("Detection Ranges")]
    public float detectionRange = 5f; // Player detection range
    public float baitDetectionRange = 7f; // Bait detection range (usually larger)
    public float eatingRange = 2f;
    
    [Header("Combat Stats")]
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    public string ID => name.Replace(" ", "");

    private void OnValidate()
    {
        palmonName = name;
    }
}