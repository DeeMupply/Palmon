using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float chaseWaitTime = 3f;
    [SerializeField] private Transform player;
    [SerializeField] private float arrivalThreshold = 1.5f; // Distance to consider "arrived" at circle position

    // Animation state constants
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_WALK = "Walk";
    private const string ANIM_RUN = "Run";

    private int currentPatrolIndex = -1;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    [SerializeField] private int myChaseIndex = -1; // This enemy's index in the chase formation
    private Vector3 targetChasePosition; // Position around player this enemy should chase
    private bool hasReachedCirclePosition = false; // Track if enemy reached their circle spot

    private enum EnemyState
    {
        Patrolling,
        Chasing,
        Searching,
        WaitingAtPatrol,
        CircleIdle // New state for when enemy reached circle position
    }

    private EnemyState currentState = EnemyState.Patrolling;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Subscribe to player events
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnChasingEnemyCountChanged += OnChasingCountChanged;
            PlayerController.Instance.OnPlayerMoved += OnPlayerMoved;
        }

        // Debug checks
        Debug.Log($"Agent found: {agent != null}");
        Debug.Log($"Agent enabled: {agent.enabled}");
        Debug.Log($"Agent on NavMesh: {agent.isOnNavMesh}");
        Debug.Log($"Animator found: {animator != null}");
        Debug.Log($"Patrol points count: {patrolPoints.Count}");

        agent.updateRotation = true;
        agent.updatePosition = true;

        if (patrolPoints.Count > 0)
        {
            Debug.Log("Starting patrol...");
            MoveToRandomPatrolPoint();
        }
        else
        {
            Debug.LogWarning("No patrol points assigned!");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnChasingEnemyCountChanged -= OnChasingCountChanged;
            PlayerController.Instance.OnPlayerMoved -= OnPlayerMoved;
        }
    }

    private void OnChasingCountChanged(int removedIndex)
    {
        // Update chase position when enemy count changes
        if ((currentState == EnemyState.Chasing || currentState == EnemyState.CircleIdle) && myChaseIndex != -1)
        {
            if (removedIndex < myChaseIndex)
            {
                myChaseIndex--; // Shift index down if an earlier enemy was removed
            }
            UpdateChasePosition();
            hasReachedCirclePosition = false; // Reset since position changed
            if (currentState == EnemyState.CircleIdle)
            {
                currentState = EnemyState.Chasing; // Go back to chasing new position
                if (animator != null)
                    animator.Play(ANIM_RUN);
            }
        }
    }

    private void OnPlayerMoved()
    {
        // Update chase position when player moves
        if ((currentState == EnemyState.Chasing || currentState == EnemyState.CircleIdle) && myChaseIndex != -1)
        {
            UpdateChasePosition();
            hasReachedCirclePosition = false; // Reset since position changed
            if (currentState == EnemyState.CircleIdle)
            {
                currentState = EnemyState.Chasing; // Go back to chasing new position
                if (animator != null)
                    animator.Play(ANIM_RUN);
            }
        }
    }

    private void UpdateChasePosition()
    {
        if (PlayerController.Instance != null && myChaseIndex != -1)
        {
            targetChasePosition = PlayerController.Instance.GetNewPositionAroundPlayer(myChaseIndex);
            agent.SetDestination(targetChasePosition);
        }
    }

    void Update()
    {
        // Debug current state
        if (Time.frameCount % 60 == 0) // Log every 60 frames
        {
            Debug.Log($"Current State: {currentState}, Agent on NavMesh: {agent.isOnNavMesh}, Has Path: {agent.hasPath}, Chase Index: {myChaseIndex}");
        }

        if (player != null && agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if player is in detect range
            if (distanceToPlayer <= detectRange)
            {
                if (currentState != EnemyState.Chasing && currentState != EnemyState.CircleIdle)
                {
                    StartChasing();
                }
                else if (currentState == EnemyState.Chasing || currentState == EnemyState.CircleIdle)
                {
                    HandleChasing();
                }
            }
            else if (currentState == EnemyState.Chasing || currentState == EnemyState.CircleIdle)
            {
                StopChasing();
            }
        }

        // Handle different states
        if (agent.isOnNavMesh)
        {
            switch (currentState)
            {
                case EnemyState.Patrolling:
                    HandlePatrolling();
                    break;

                case EnemyState.Searching:
                    HandleSearching();
                    break;

                case EnemyState.WaitingAtPatrol:
                    HandleWaitingAtPatrol();
                    break;

                case EnemyState.CircleIdle:
                    HandleCircleIdle();
                    break;
            }
        }
    }

    private void HandleChasing()
    {
        if (myChaseIndex != -1)
        {
            // Check if we've reached the circle position
            float distanceToTarget = Vector3.Distance(transform.position, targetChasePosition);
            if (distanceToTarget <= arrivalThreshold && !hasReachedCirclePosition)
            {
                hasReachedCirclePosition = true;
                currentState = EnemyState.CircleIdle;
                
                // Stop moving and face the player
                agent.ResetPath();
                
                // Play idle animation
                if (animator != null)
                    animator.Play(ANIM_IDLE);
                
                Debug.Log("Reached circle position, switching to idle and facing player");
            }
            else if (!hasReachedCirclePosition)
            {
                // Continue moving to target position
                agent.SetDestination(targetChasePosition);
            }
        }
    }

    private void HandleCircleIdle()
    {
        // Face the player while in circle position
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0; // Keep only horizontal rotation
            
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    private void StartChasing()
    {
        Debug.Log("Starting to chase player!");
        currentState = EnemyState.Chasing;
        isWaiting = false;
        hasReachedCirclePosition = false;

        // Register with PlayerController and get chase index
        if (PlayerController.Instance != null)
        {
            myChaseIndex = PlayerController.Instance.AddEnemyCount();
            UpdateChasePosition();
        }

        // Play run animation
        if (animator != null)
            animator.Play(ANIM_RUN);
    }

    private void StopChasing()
    {
        Debug.Log("Lost player, searching...");
        
        // Unregister from PlayerController
        if (PlayerController.Instance != null && myChaseIndex != -1)
        {
            int oldChaseIndex = myChaseIndex;
            myChaseIndex = -1;
            PlayerController.Instance.RemoveEnemyCount(oldChaseIndex);
        }

        hasReachedCirclePosition = false;
        currentState = EnemyState.Searching;
        waitTimer = 0f;

        // Play idle animation while searching
        if (animator != null)
            animator.Play(ANIM_IDLE);
    }

    private void HandlePatrolling()
    {
        // Check if we've reached the destination
        if (agent.isActiveAndEnabled && !agent.pathPending)
        {
            // If we don't have a path OR we've reached the destination
            if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!isWaiting)
                {
                    Debug.Log("Reached patrol point, waiting...");
                    isWaiting = true;
                    waitTimer = 0f;
                    currentState = EnemyState.WaitingAtPatrol;
                    
                    // Play idle animation while waiting
                    if (animator != null)
                        animator.Play(ANIM_IDLE);
                }
            }
        }
    }

    private void HandleWaitingAtPatrol()
    {
        waitTimer += Time.deltaTime;

        if (waitTimer >= patrolWaitTime)
        {
            Debug.Log("Wait finished, moving to next patrol point...");
            isWaiting = false;
            MoveToRandomPatrolPoint();
            currentState = EnemyState.Patrolling;
            
            // Play walk animation when starting to patrol
            if (animator != null)
                animator.Play(ANIM_WALK);
        }
    }

    private void HandleSearching()
    {
        waitTimer += Time.deltaTime;

        if (waitTimer >= chaseWaitTime)
        {
            Debug.Log("Search finished, returning to patrol...");
            currentState = EnemyState.Patrolling;
            MoveToRandomPatrolPoint();
            
            // Play walk animation when returning to patrol
            if (animator != null)
                animator.Play(ANIM_WALK);
        }
    }

    private void ChasePlayer()
    {
        if (agent.isOnNavMesh && myChaseIndex != -1)
        {
            // Chase the assigned position around the player
            agent.SetDestination(targetChasePosition);
        }
    }

    private void MoveToRandomPatrolPoint()
    {
        if (patrolPoints.Count == 0 || !agent.isOnNavMesh)
        {
            Debug.LogWarning("Cannot move: No patrol points or agent not on NavMesh");
            return;
        }

        int newIndex;

        if (patrolPoints.Count == 1)
        {
            newIndex = 0;
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, patrolPoints.Count);
            }
            while (newIndex == currentPatrolIndex);
        }

        currentPatrolIndex = newIndex;
        Vector3 destination = patrolPoints[currentPatrolIndex].position;

        Debug.Log($"Moving to patrol point {newIndex} at position {destination}");

        bool pathSet = agent.SetDestination(destination);
        Debug.Log($"Path set successfully: {pathSet}");
        
        // Play walk animation when moving to patrol point
        if (animator != null)
            animator.Play(ANIM_WALK);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Draw target chase position
        if ((currentState == EnemyState.Chasing || currentState == EnemyState.CircleIdle) && myChaseIndex != -1)
        {
            Gizmos.color = hasReachedCirclePosition ? Color.green : Color.red;
            Gizmos.DrawWireSphere(targetChasePosition, 0.3f);
            Gizmos.DrawLine(transform.position, targetChasePosition);
            
            // Draw arrival threshold
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(targetChasePosition, arrivalThreshold);
        }

        // Draw patrol points
        if (patrolPoints != null)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawWireSphere(patrolPoints[i].position, 0.5f);
                    if (i == currentPatrolIndex)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(patrolPoints[i].position, 0.7f);
                        Gizmos.color = Color.blue;
                    }
                }
            }
        }
    }

    [ContextMenu("Kill Enemy")]
    public void KillEnemy()
    {
        // Unregister from PlayerController if chasing
        if (PlayerController.Instance != null && myChaseIndex != -1)
        {
            int oldChaseIndex = myChaseIndex;
            myChaseIndex = -1;
            PlayerController.Instance.RemoveEnemyCount(oldChaseIndex);
        }

        Debug.Log("Enemy killed!");
        Destroy(gameObject);
    }
}