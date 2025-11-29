using UnityEngine;
using System;

public partial class Player : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    // Battle-related properties and methods can be added here in the future
    [Header("Circle Formation")]
    [SerializeField] private float circleRadius = 3f; // Radius of the circle around player

    // Enemy tracking
    [SerializeField] private int chasingEnemyCount = 0;
    public event Action<int> OnChasingEnemyCountChanged;
    public event Action OnPlayerMoved;

    #region Enemy Formation System

    public int AddEnemyCount()
    {
        chasingEnemyCount++;
        Debug.Log($"Chasing enemy count increased: {chasingEnemyCount}");
        OnChasingEnemyCountChanged?.Invoke(chasingEnemyCount);
        return chasingEnemyCount - 1; // Return the index for this enemy
    }

    public void RemoveEnemyCount(int removedIndex)
    {
        chasingEnemyCount = Math.Max(0, chasingEnemyCount - 1);
        Debug.Log($"Chasing enemy count decreased: {chasingEnemyCount}");
        OnChasingEnemyCountChanged?.Invoke(removedIndex);
    }

    public Vector3 GetNewPositionAroundPlayer(int index)
    {
        if (chasingEnemyCount <= 0) return transform.position;

        // Calculate angle for this enemy based on its index
        float angleStep = 360f / chasingEnemyCount;
        float angle = index * angleStep;

        // Convert angle to radians
        float angleInRadians = angle * Mathf.Deg2Rad;

        // Calculate position around player
        Vector3 offset = new Vector3(
            Mathf.Cos(angleInRadians) * circleRadius,
            0f,
            Mathf.Sin(angleInRadians) * circleRadius
        );

        return transform.position + offset;
    }

    #endregion
}