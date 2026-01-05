using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class Palmon : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    [Header("Palmon Specific Data")]
    [SerializeField] private PalmonSO palmonData;
    [SerializeField] private PalmonAnimation palmonAnimation;
    [SerializeField] private List<Palmon_OnEvent_Behaviour> palmonEventBehaviours;
    [SerializeField] private List<Palmon_OnEvent_Behaviour> secondaryEventBehaviours;

    [SerializeField] private bool isDrawingGizmos = true;
    public bool IsUsingNavMeshAgent { get; set; } = true;
    private Transform target;
    private Vector2 attackPosition;

    private readonly float navigationUpdateInterval = 0.5f; // seconds
    private float navigationUpdateTimer = 0f;
    private bool hasJustAttacked = false;
    private bool isSpecialAttacking = false;

    public void StartSpecialAttack()
    {
        isSpecialAttacking = true;
    }
    public void EndSpecialAttack()
    {
        isSpecialAttacking = false;
    }

    #region Unity Lifecycle
    private void Start()
    {
        target = Player.Instance.GetTransform();
        if (palmonData != null)
        {
            navMeshAgent.speed = palmonData.walkSpeed;
        }
        // Define state transitions
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Eating, ShouldEating));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.RotateWithoutMoving, ShouldRotateWithoutMoving));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Attacking, ShouldAttacking));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Running, ShouldRunning));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Moving, ShouldMoving));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Idle, ShouldIdle));

        UpdateAttackPosition();
    }

    private void Update()
    {
        UpdateTarget(); // Add this line at the top

        HandleUpdateAttackPosition();
        if (currentState == PalmonState.Moving || currentState == PalmonState.Running)
        {
            navigationUpdateTimer += Time.deltaTime;
            if (navigationUpdateTimer >= navigationUpdateInterval)
            {
                UpdateNavigationTarget();
                navigationUpdateTimer = 0f;
            }
        }
        else
        {
            navigationUpdateTimer = 0f;
        }

        HandlePalmonStates();
        RotateWithoutMoving();
        CheckBaitConsumption(); // Add this line at the end
    }

    private void HandleUpdateAttackPosition()
    {
        if (currentState != PalmonState.Moving && currentState != PalmonState.Running) return;
        if (target == null) return;

        // Only update attack position for Player targets
        if (!target.TryGetComponent(out Player _)) return;

        float distanceToTarget = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(target.position.x, target.position.z)
        );

        if (distanceToTarget < palmonData.attackRange)
        {
            UpdateAttackPosition();
        }
    }

    // Update target slower than every frame
    private void UpdateNavigationTarget()
    {
        navMeshAgent.SetDestination(target.position);
    }

    public void UpdateAttackPosition()
    {
        if (target == null) return;
        attackPosition = new Vector2(target.position.x, target.position.z);
    }

    void OnDrawGizmos()
    {
        if (!isDrawingGizmos) return;
        if (palmonData != null)
        {
            // Visualize the attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, palmonData.attackRange);

            // Visualize the player detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, palmonData.detectionRange);

            // Visualize the bait detection range
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, palmonData.baitDetectionRange);

            // Visualize the eating range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, palmonData.eatingRange);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(attackPosition.x, transform.position.y, attackPosition.y));
    }

    #endregion

    #region Universal Palmon State Behaviours
    private void StopMoving()
    {
        if (!IsUsingNavMeshAgent) return;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
    }

    private void RotateWithoutMoving()
    {
        if (attackPosition == null) return;
        if (currentState != PalmonState.RotateWithoutMoving) return;
        Vector2 direction = (attackPosition - new Vector2(transform.position.x, transform.position.z)).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * palmonData.rotationSpeed);
    }

    #endregion

    #region Public Methods
    public void DeactivateNavMeshAgent()
    {
        if (navMeshAgent != null)
        {
            IsUsingNavMeshAgent = false;
            navMeshAgent.enabled = false;
            // navMeshObstacle.enabled = true;
        }
    }

    public void ReactivateNavMeshAgent(Vector3 position)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(position);
            navMeshAgent.enabled = true;
            IsUsingNavMeshAgent = true;
        }
    }

    public void PalmonSpecialRunAttack()
    {
        IsUsingNavMeshAgent = true;
        navMeshAgent.isStopped = false;
    }

    public List<Palmon_OnEvent_Behaviour> GetSecondaryEventBehaviours()
    {
        return secondaryEventBehaviours;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return navMeshAgent;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector2 GetAttackPosition()
    {
        return attackPosition;
    }

    public PalmonState GetCurrentState()
    {
        return currentState;
    }

    public PalmonAnimation GetPalmonAnimation()
    {
        return palmonAnimation;
    }

    public float GetAttackDamage()
    {
        if (palmonData != null)
        {
            return palmonData.attackDamage;
        }
        return 0f;
    }

    public float GetRunSpeed()
    {
        if (palmonData != null)
        {
            return palmonData.runSpeed;
        }
        return 0f;
    }

    public void UpdateHasJustAttackedAfterAttacking()
    {
        hasJustAttacked = true;
        StartCoroutine(ResetHasJustAttackedAfterDelay(0.5f));
    }

    private IEnumerator ResetHasJustAttackedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateAttackPosition();
        hasJustAttacked = false;
    }

    public List<Palmon_OnEvent_Behaviour> GetPalmonEventBehaviours(PalmonState state)
    {
        return palmonEventBehaviours.FindAll(behaviour => behaviour.AssociatedState == state);
    }

    #endregion

    [ContextMenu("Log Palmon Nav Mesh Agent")]
    private void LogNavMeshAgent()
    {
        Debug.Log($"Is NavMeshAgent stopped: {navMeshAgent.isStopped}");
        Debug.Log($"NavMeshAgent target position: {navMeshAgent.destination}");
    }
}