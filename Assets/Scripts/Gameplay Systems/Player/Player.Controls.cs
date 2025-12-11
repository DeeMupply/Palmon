using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public partial class Player
{
    [SerializeField] private CinemachineInputAxisController cameraAxisController;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Jump & Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask = 1;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float jumpBufferTime = 0.1f; // Buffer time for jump input

    // Jump mechanics
    private int jumpCount = 0;
    private bool jumpInputPressed = false;
    private float jumpInputTime = 0f;
    private bool wasGroundedLastFrame;
    private bool hasJumpedThisFrame = false;



    [SerializeField] private Transform cameraTransform; // Assign your Cinemachine camera's transform in Inspector

    // Components
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInputActions playerInputActions;

    // Movement variables
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isControllingCursor = false;
    private bool isControllingPlayer = true;
    public event Action OnScansSubmitted;

    private Transform targetingPalmon;

    #region Update Methods

    private void HandleMovementWithSprinting()
    {
        // Store previous grounded state
        wasGroundedLastFrame = isGrounded;

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) && characterController.velocity.y <= 0.1f;

        // Reset jump count when landing (not every frame on ground)
        if (isGrounded && !wasGroundedLastFrame)
        {
            jumpCount = 0;
            isJumping = false;
        }

        // Handle jump input with buffering
        HandleJumpLogic();

        // Apply gravity before movement
        if (!isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

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

        // Apply horizontal movement
        float currentSpeed = (isSprinting && currentStamina > 0) ? sprintSpeed : moveSpeed;
        characterController.Move(currentSpeed * Time.deltaTime * moveDir);

        // Apply vertical movement
        characterController.Move(velocity * Time.deltaTime);

        // Reset jump flag at end of frame
        hasJumpedThisFrame = false;
    }

    private void HandleJumpLogic()
    {
        // Check if we have buffered jump input
        bool hasJumpBuffer = jumpInputPressed && (Time.time - jumpInputTime) <= jumpBufferTime;

        if (hasJumpBuffer && !hasJumpedThisFrame)
        {
            // Can jump if grounded (first jump) or haven't used all air jumps
            bool canJump = isGrounded ? jumpCount < 1 : jumpCount < maxJumps;

            if (canJump)
            {
                PerformJump();
                jumpInputPressed = false; // Consume the jump input
            }
        }

        // Clear old jump input
        if (jumpInputPressed && (Time.time - jumpInputTime) > jumpBufferTime)
        {
            jumpInputPressed = false;
        }
    }

    private void PerformJump()
    {
        // Apply jump velocity
        velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);

        // Increment jump count
        jumpCount++;

        // Set flags
        isJumping = true;
        hasJumpedThisFrame = true;

        Debug.Log($"Player jumped! Jump count: {jumpCount}");
    }

    #endregion

    #region Input Callbacks

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isUsingTool) return; // Prevent movement while using a tool
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput.sqrMagnitude > 0.01f;
        OnPlayerMoved?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (!isGrounded && jumpCount >= maxJumps) return; // No jumps left
        if (isUsingTool) return; // Prevent jumping while using a tool
        if (context.performed)
        {
            // Store jump input with timestamp for buffering
            jumpInputPressed = true;
            jumpInputTime = Time.time;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isUsingTool) return; // Prevent sprinting while using a tool
        if (context.started)
        {
            if (currentStamina > 0)
                isSprinting = true;
        }
        else if (context.canceled)
        {
            isSprinting = false;
        }
    }

    public void OnTool1(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isUsingTool) return; // Prevent switching tools while using one
        if (context.performed)
        {
            Debug.Log("Tool 1 activated.");
            SwitchToTool(tools[1].ID);
        }
    }

    public void OnTool2(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isUsingTool) return; // Prevent switching tools while using one
        if (context.performed)
        {
            Debug.Log("Tool 2 activated.");
            SwitchToTool(tools[2].ID);
        }
    }

    public void OnTool3(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isUsingTool) return; // Prevent switching tools while using one
        if (context.performed)
        {
            Debug.Log("Tool 3 activated.");
            SwitchToTool(tools[3].ID);
        }
    }

    public void OnTool4(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isUsingTool) return; // Prevent switching tools while using one
        if (context.performed)
        {
            Debug.Log("Tool 4 activated.");
            SwitchToTool(tools[4].ID);
        }
    }

    public void OnToolScan(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isUsingTool) return; // Prevent switching tools while using one
        if (context.performed)
        {
            Debug.Log("Tool Scan activated.");
            SwitchToTool(tools[0].ID);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isControllingCursor) return;
        if (!isGrounded) return; // Prevent using tools mid-air
        if (isUsingTool) return; // Prevent spamming tool use
        if (ToolDictionary[currentToolID].IsOnCooldown)
        {
            Debug.Log("Tool is on cooldown.");
            return;
        }
        if (context.performed)
        {
            Debug.Log($"Using current tool: {currentToolID}");
            isUsingTool = true;
        }
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        Debug.LogWarning("Targeting system not implemented yet.");
    }

    public void OnCursorToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CursorToggleOn();
        }
        else if (context.canceled)
        {
            CursorToggleOff();
        }
    }

    public void OnEscapePress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePauseState();
            CanvasMainGame.Instance.TogglePausedMenu();
        }
    }

    public void TogglePauseState()
    {
        isControllingPlayer = !isControllingPlayer;
        if (isControllingPlayer)
        {
            CursorToggleOff();
        }
        else
        {
            CursorToggleOn();
        }
    }

    private void CursorToggleOn()
    {
        isControllingCursor = true;
        cameraAxisController.enabled = false;
        CursorController.Instance.ShowCursor();
    }

    public void CursorToggleOff()
    {
        isControllingCursor = false;
        cameraAxisController.enabled = true;
        CursorController.Instance.HideCursor();
    }

    private void ReplenishAndSubmitScans()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina);
        ReplenishAllTools();
        OnScansSubmitted?.Invoke();
    }

    /*
    private void PauseCameraRotation()
    {
        cameraAxisController.enabled = false;
    }
    private void ResumeCameraRotation()
    {
        cameraAxisController.enabled = true;
    }
    */

    #endregion
}