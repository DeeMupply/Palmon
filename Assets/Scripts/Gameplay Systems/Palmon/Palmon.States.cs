using System;
using System.Collections.Generic;
using UnityEngine;


public partial class Palmon
{
    // Internal fields
    [SerializeField] private PalmonState currentState;
    // Implement palmon states and their transition conditions
    private List<PalmonStateTransition> stateTransitions = new List<PalmonStateTransition>();

    #region Palmon State Management
    private void HandlePalmonStates()
    {
        for (int i = 0; i < stateTransitions.Count; i++)
        {
            PalmonStateTransition stateTransition = stateTransitions[i];
            if (stateTransition == null)
            {
                Debug.LogWarning("State transition is null at index: " + i);
                continue;
            }

            if (stateTransition.ShouldTransition() && currentState != stateTransition.NewState)
            {
                OnPalmonStateChange(stateTransition.NewState);
                return;
            }
        }
        // Debug.LogWarning("No valid state transition found for current state: " + currentState);
    }

    private void OnPalmonStateChange(PalmonState newState)
    {
        currentState = newState;
        UpdateAttackPosition();

        switch (newState)
        {
            case PalmonState.Idle:
                StopMoving();
                break;
            case PalmonState.Moving:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = palmonData.walkSpeed;
                navMeshAgent.SetDestination(target.position);
                break;
            case PalmonState.Running:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = palmonData.runSpeed;
                navMeshAgent.SetDestination(target.position);
                break;
            case PalmonState.Attacking:
                StopMoving();
                break;
            case PalmonState.RotateWithoutMoving:
                StopMoving();
                break;
            case PalmonState.Eating:
                StopMoving();
                break;
        }

        palmonAnimation.OnPalmonStateChange(newState, this);
    }


    #endregion

    #region Should State Transition Methods

    private bool ShouldEating()
    {
        if (isSpecialAttacking) return false;
        if (target == null) return false;
        if (currentState == PalmonState.Attacking) return false;

        // Check if targeting bait specifically
        if (!target.TryGetComponent(out Bait _)) return false;

        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z));
        if (distanceToTarget <= palmonData.eatingRange)
        {
            return true;
        }
        return false;
    }

    private bool ShouldRotateWithoutMoving()
    {
        if (currentState == PalmonState.Eating) return false;
        if (isSpecialAttacking) return false;
        if (target == null) return false;
        if (currentState == PalmonState.Attacking) return false;

        // Only rotate when targeting Player for attacking
        if (!target.TryGetComponent(out Player _)) return false;

        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), attackPosition);
        // Don't rotate if too close or too far
        if (distanceToTarget < 0.01f) return false;
        if (distanceToTarget > palmonData.attackRange) return false;

        // Project onto XZ plane
        Vector2 toTarget = attackPosition - new Vector2(transform.position.x, transform.position.z);
        Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
        float angleToTarget = Vector2.Angle(forward, toTarget.normalized);

        if (angleToTarget <= palmonData.rotationThresholdAngle) return false;
        return true;
    }

    private bool ShouldAttacking()
    {
        if (currentState == PalmonState.Eating) return false;
        if (hasJustAttacked) return false;
        if (target == null) return false;

        // Only attack when targeting Player
        if (!target.TryGetComponent(out Player _)) return false;

        // Just attack if very close
        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), attackPosition);
        if (distanceToTarget < 0.01f) return true;

        // Check distance threshold
        if (distanceToTarget > palmonData.attackRange) return false;

        // Check angle threshold if within attack range
        Vector2 toTarget = attackPosition - new Vector2(transform.position.x, transform.position.z);
        Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
        float angleToTarget = Vector2.Angle(forward, toTarget.normalized);
        if (angleToTarget > palmonData.rotationThresholdAngle) return false;

        return true;
    }

    private bool ShouldRunning()
    {
        if (currentState == PalmonState.Eating) return false;
        if (isSpecialAttacking) return false;
        if (target == null) return false;

        // Only run when targeting Player
        if (!target.TryGetComponent(out Player _)) return false;

        if (!IsUsingNavMeshAgent) return false;
        if (currentState == PalmonState.Attacking) return false;

        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z));
        if (distanceToTarget > palmonData.detectionRange)
        {
            return false;
        }
        if (distanceToTarget > palmonData.attackRange && distanceToTarget <= palmonData.detectionRange)
        {
            return true;
        }
        return false;
    }

    private bool ShouldMoving()
    {
        if (currentState == PalmonState.Eating) return false;
        if (isSpecialAttacking) return false;
        if (target == null) return false;

        // Walk to bait (not running)
        if (!target.TryGetComponent(out Player _))
        {
            if (!IsUsingNavMeshAgent) return false;
            if (currentState == PalmonState.Attacking) return false;
            if (currentState == PalmonState.Eating) return false;

            float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z));

            // Move to bait if far enough
            if (distanceToTarget > palmonData.eatingRange)
            {
                return true;
            }
        }

        return false;
    }

    private bool ShouldIdle()
    {
        if (currentState == PalmonState.Eating) return false;
        if (isSpecialAttacking) return false;
        if (target == null) return true;
        if (hasJustAttacked) return true;

        // If targeting player, check player detection range
        if (target.TryGetComponent(out Player _))
        {
            float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z));
            if (distanceToTarget > palmonData.detectionRange)
            {
                return true;
            }
        }

        return false;
    }

    public class PalmonStateTransition
    {
        public PalmonState NewState { get; private set; }
        public Func<bool> ShouldTransition { get; private set; }
        public PalmonStateTransition(PalmonState newState, Func<bool> shouldTransition)
        {
            NewState = newState;
            ShouldTransition = shouldTransition;
        }
    }

    #endregion
}


public enum PalmonState
{
    Idle,
    RotateWithoutMoving,
    Moving,
    Running,
    Attacking,
    Eating,
    None
}