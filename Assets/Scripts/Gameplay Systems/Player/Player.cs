using System.ComponentModel;
using UnityEngine;

public partial class Player : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.SetCallbacks(this);
            playerInputActions.Player.Enable();
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
            Debug.LogWarning("CharacterController was not assigned. Fetched from GameObject.");
        }

        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck transform was not assigned. Created a new one.");
        }
        InitializeTools();
    }

    private void OnDestroy()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Disable();
            playerInputActions.Dispose();
        }
    }

    private void Update()
    {
        HandleMovementWithSprinting();
        HandleStamina();
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