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

    // Internal fields
    [SerializeField] private PalmonState currentState;
    // Implement palmon states and their transition conditions
    private List<PalmonStateTransition> stateTransitions = new List<PalmonStateTransition>();

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
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Dying, ShouldDying));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Hit, ShouldHit));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.RotateWithoutMoving, ShouldRotateWithoutMoving));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Attacking, ShouldAttacking));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Running, ShouldRunning));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Moving, ShouldMoving));
        stateTransitions.Add(new PalmonStateTransition(PalmonState.Idle, ShouldIdle));

        UpdateAttackPosition();
    }

    private void Update()
    {
        HandleUpdateAttackPosition();
        if (currentState == PalmonState.Moving)
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
            navigationUpdateTimer = 0f; // reset timer if not moving
        }

        HandlePalmonStates();
        RotateWithoutMoving();
    }

    private void HandleUpdateAttackPosition()
    {
        if (currentState != PalmonState.Moving) return;
        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z));
        if (distanceToTarget < palmonData.attackRange)
        {
            // Lock attack position to target's current position
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

            // Visualize the detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, palmonData.detectionRange);
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
            case PalmonState.Hit:
                // Implement later
                break;
            case PalmonState.Dying:
                StopMoving();
                // Implement later: disable palmon
                break;
        }

        palmonAnimation.OnPalmonStateChange(newState, this);
    }


    #endregion

    #region Should State Transition Methods

    private bool ShouldDying()
    {
        // Implement later: check health <= 0
        if (currentState == PalmonState.Attacking) return false;
        return false;
    }
    private bool ShouldHit()
    {
        // Implement later: check if hit by player
        if (currentState == PalmonState.Attacking) return false;
        return false;
    }

    private bool ShouldRotateWithoutMoving()
    {
        if (isSpecialAttacking) return false;
        if (target == null) return false;
        if (currentState == PalmonState.Attacking) return false;

        // Target locked to attack position

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
        if (hasJustAttacked) return false;
        // Target locked to attack position

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
        if (isSpecialAttacking) return false;
        if (target == null) return false;
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
        if (isSpecialAttacking) return false;
        if (target == null) return false;
        if (target.TryGetComponent(out Player _)) return false;
        if (!IsUsingNavMeshAgent) return false;
        if (currentState == PalmonState.Attacking) return false;

        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z));
        if (distanceToTarget > palmonData.attackRange && distanceToTarget <= palmonData.detectionRange)
        {
            return true;
        }
        return false;
    }

    private bool ShouldIdle()
    {
        if (isSpecialAttacking) return false;
        if (target == null) return true;
        if (hasJustAttacked) return true;
        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.position.x, target.position.z));
        if (distanceToTarget > palmonData.detectionRange)
        {
            return true;
        }
        return false;
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

    [ContextMenu("Log Palmon Nav Mesh Agent")]
    private void LogNavMeshAgent()
    {
        Debug.Log($"Is NavMeshAgent stopped: {navMeshAgent.isStopped}");
        Debug.Log($"NavMeshAgent target position: {navMeshAgent.destination}");
    }

    private enum TargetDistanceCategory
    {
        OutOfRange,
        InDetectionRange,
        InAttackRange,
        VeryClose
    }
}

public enum PalmonState
{
    Idle,
    RotateWithoutMoving,
    Moving,
    Running,
    Attacking,
    Hit,
    Dying
}