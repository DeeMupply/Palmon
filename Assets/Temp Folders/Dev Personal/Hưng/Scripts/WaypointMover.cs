using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotateSpeed = 8f;
    [SerializeField] private float distanceThreshold = 0.1f;

    private Transform currentWaypoint;
    private bool canMove = true;

    void Start()
    {
        currentWaypoint = waypoints.GetNextWaypoint(null);
        RotateTowards(currentWaypoint.position);
    }

    void Update()
    {
        if (!canMove || currentWaypoint == null) return;

        // Move
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentWaypoint.position,
            moveSpeed * Time.deltaTime
        );

        // Check arrival
        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        }

        // Rotate smoothly
        RotateTowards(currentWaypoint.position);
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );
    }

    // ===== Called by QuizManager =====
    public void StopMovement()
    {
        canMove = false;
    }

    public void ResumeMovement()
    {
        canMove = true;
    }
}
