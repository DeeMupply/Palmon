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
    [SerializeField] private int jumpCount = 0;

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

    private Transform targetingPalmon;

    #region Update Methods

    private void HandleMovementWithSprinting()
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

        float currentSpeed = (isSprinting && currentStamina > 0) ? sprintSpeed : moveSpeed;
        characterController.Move(currentSpeed * Time.deltaTime * moveDir);

        // Gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    #endregion

    #region Input Callbacks

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput.sqrMagnitude > 0.01f;
        OnPlayerMoved?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (context.performed && jumpCount < maxJumps - 1)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            jumpCount++;
            Debug.Log("Player jumped!");
            isJumping = true;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
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
        if (context.performed)
        {
            Debug.Log("Tool 1 activated.");
            currentToolID = tools[1].ID;
            OnCurrentToolChanged?.Invoke();
        }
    }

    public void OnTool2(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (context.performed)
        {
            Debug.Log("Tool 2 activated.");
            currentToolID = tools[2].ID;
            OnCurrentToolChanged?.Invoke();
        }
    }

    public void OnTool3(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (context.performed)
        {
            Debug.Log("Tool 3 activated.");
            currentToolID = tools[3].ID;
            OnCurrentToolChanged?.Invoke();
        }
    }

    public void OnTool4(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (context.performed)
        {
            Debug.Log("Tool 4 activated.");
            currentToolID = tools[4].ID;
            OnCurrentToolChanged?.Invoke();
        }
    }

    public void OnToolScan(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (context.performed)
        {
            Debug.Log("Tool Scan activated.");
            currentToolID = tools[0].ID; // Switch to first tool. It's the scan tool.
            OnCurrentToolChanged?.Invoke();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!isControllingPlayer) return;
        if (isControllingCursor) return;
        if (context.performed)
        {
            Debug.Log($"Using current tool: {currentToolID}");
            ToolDictionary[currentToolID].UseTool();
            if (currentToolID == tools[0].ID)
            {
                isUsingScanTool = true;
            }
            else
            {
                isUsingTool = true;
            }
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
            isControllingPlayer = false;
            CursorToggleOn();
            CanvasMainGame.Instance.ShowPausedMenu();
        }
    }

    public void OnResumeFromPause()
    {
        isControllingPlayer = true;
        CursorToggleOff();
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