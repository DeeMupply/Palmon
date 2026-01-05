using UnityEngine;

public partial class Palmon
{
    private enum TargetType
    {
        None,
        Player,
        Bait
    }

    [SerializeField] private TargetType currentTargetType = TargetType.None;
    [SerializeField] private Bait currentBaitTarget = null;
    [SerializeField] private bool isEatingBait = false;

    private void UpdateTarget()
    {
        // Don't change target while eating
        if (isEatingBait && currentBaitTarget != null) return;

        // Priority 1: Check if we should target player
        if (ShouldTargetPlayer())
        {
            SetTargetToPlayer();
            return;
        }

        // Priority 2: Check for nearby bait that has space (using baitDetectionRange)
        Bait nearestBait = BaitManager.Instance?.GetNearestBait(transform.position, palmonData.baitDetectionRange);
        if (nearestBait != null && nearestBait.CanEat())
        {
            SetTargetToBait(nearestBait);
            return;
        }

        // Priority 3: No target
        ClearTarget();
    }

    private bool ShouldTargetPlayer()
    {
        Player player = Player.Instance;
        if (player == null) return false;

        // Don't target if player is invisible
        if (player.IsInvisible) return false;

        // Check if player is within detection range
        float distanceToPlayer = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(player.transform.position.x, player.transform.position.z)
        );

        return distanceToPlayer <= palmonData.detectionRange;
    }

    private void SetTargetToPlayer()
    {
        if (currentTargetType != TargetType.Player)
        {
            // Stop eating if we were eating bait
            if (isEatingBait && currentBaitTarget != null)
            {
                currentBaitTarget.StopEating(this);
                isEatingBait = false;
            }

            currentTargetType = TargetType.Player;
            currentBaitTarget = null;
            target = Player.Instance.GetTransform();
            Debug.Log($"{palmonData.palmonName} is now targeting Player");
        }
    }

    private void SetTargetToBait(Bait bait)
    {
        if (currentBaitTarget != bait)
        {
            // Stop eating previous bait
            if (isEatingBait && currentBaitTarget != null)
            {
                currentBaitTarget.StopEating(this);
                isEatingBait = false;
            }

            currentTargetType = TargetType.Bait;
            currentBaitTarget = bait;
            target = bait.transform;
            Debug.Log($"{palmonData.palmonName} is now targeting Bait");
        }
    }

    private void ClearTarget()
    {
        if (currentTargetType != TargetType.None)
        {
            // Stop eating if we were eating
            if (isEatingBait && currentBaitTarget != null)
            {
                currentBaitTarget.StopEating(this);
                isEatingBait = false;
            }

            currentTargetType = TargetType.None;
            currentBaitTarget = null;
            target = null;
            Debug.Log($"{palmonData.palmonName} has no target");
        }
    }

    private void CheckBaitConsumption()
    {
        if (currentTargetType != TargetType.Bait || currentBaitTarget == null) return;

        float distanceToBait = Vector3.Distance(transform.position, currentBaitTarget.GetPosition());

        // Start eating if close enough and not already eating
        if (distanceToBait <= palmonData.eatingRange && !isEatingBait)
        {
            if (currentBaitTarget.TryStartEating(this))
            {
                isEatingBait = true;
                Debug.Log($"{palmonData.palmonName} started eating bait!");
            }
        }
    }

    // Called by Bait when it's destroyed
    public void OnBaitDestroyed()
    {
        if (isEatingBait)
        {
            isEatingBait = false;
            currentBaitTarget = null;
            ClearTarget();
            Debug.Log($"{palmonData.palmonName}'s bait was destroyed!");
        }
    }
}