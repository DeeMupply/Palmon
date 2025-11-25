using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEditor;

public class PlayerController : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    public static PlayerController Instance { get; private set; }

    [Header("Circle Formation")]
    [SerializeField] private float circleRadius = 3f; // Radius of the circle around player

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask = 1;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int jumpCount = 0;

    [SerializeField] private float dashDuration = 0.2f; // How long dash lasts
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private Vector3 dashDirection;

    [SerializeField] private Transform cameraTransform; // Assign your Cinemachine camera's transform in Inspector

    // Components
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInputActions playerInputActions;

    // Movement variables
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool canDash = true;
    private float lastDashTime;

    // Enemy tracking
    [SerializeField]private int chasingEnemyCount = 0;
    public event Action<int> OnChasingEnemyCountChanged;
    public event Action OnPlayerMoved;

    private Vector3 lastPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Get components
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
            Debug.LogWarning("CharacterController was not assigned. Fetched from GameObject.");
        }

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.SetCallbacks(this);
        playerInputActions.Player.Enable();

        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckGO = new GameObject("GroundCheck");
            groundCheckGO.transform.SetParent(transform);
            groundCheckGO.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckGO.transform;
            Debug.LogWarning("GroundCheck transform was not assigned. Created a new one.");
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        HandleDashCooldown();
        HandleMovementWithDashing();
    }

    /*
    private void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

        // Horizontal movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    */

    private void HandleDashCooldown()
    {
        if (!canDash && Time.time >= lastDashTime + dashCooldown)
        {
            canDash = true;
        }
    }

    #region Input Callbacks

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        OnPlayerMoved?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && jumpCount < maxJumps - 1)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            jumpCount++;
            Debug.Log("Player jumped!");
        }
    }

    private void HandleMovementWithDashing()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            jumpCount = 0; // Reset jump count when grounded
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Camera-relative movement direction
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camRight * moveInput.x + camForward * moveInput.y;

        // Instantly rotate character to movement direction if moving
        if (moveInput.sqrMagnitude > 0.01f && moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDir);
        }

        Vector3 move;
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
            move = dashDirection * dashForce;
        }
        else
        {
            move = moveDir * moveSpeed;
        }

        characterController.Move(move * Time.deltaTime);

        // Gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    // Update your OnDash callback:
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && isGrounded && !isDashing)
        {
            Vector3 targetDashDirection = transform.forward * moveInput.y;
            if (targetDashDirection == Vector3.zero)
                targetDashDirection = transform.forward;

            targetDashDirection.y = 0;
            targetDashDirection.Normalize();

            isDashing = true;
            dashTimeLeft = dashDuration;
            dashDirection = targetDashDirection;

            canDash = false;
            lastDashTime = Time.time;
            OnPlayerMoved?.Invoke();

            Debug.Log("Player dashed!");
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        // We'll implement camera look later
    }

    #endregion

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

    private void OnDestroy()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Disable();
            playerInputActions.Dispose();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the circle radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, circleRadius);

        // Visualize ground check
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}