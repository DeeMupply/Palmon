using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    public static Player Instance { get; private set; }
    [SerializeField] private Vector3 respawnPosition;
    [SerializeField] private Quaternion respawnRotation;

    public event System.Action OnPlayerWin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.SetCallbacks(this);
            playerInputActions.Player.Enable();
            InitializeTools();
            InitScanHitBox();
            InitializeInvisibility();
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
        InitializeStatus();
        InitializeIngameToolObjectReferences();
        InitializeCurrentTool();
        LoadToolData(GameManager.Instance.GetActiveToolSaveData());
        LoadPlayerSaveData(GameManager.Instance.GetActivePlayerSaveData());
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
        UpdateAnimation();
        UpdateToolCooldowns();
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

    public void Respawn()
    {
        // Reset Animation
        ResetAllAnimationFlags();
        animator.speed = 1f;

        // Move player to respawn position
        characterController.enabled = false;
        transform.SetLocalPositionAndRotation(respawnPosition, respawnRotation);
        characterController.enabled = true;
        velocity = Vector3.zero;

        // Reset Status
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        OnHealthChanged?.Invoke(currentHealth);
        OnStaminaChanged?.Invoke(currentStamina);
        ReplenishAllTools();

        SetPauseState(false);
    }

    public Transform GetTransform()
    {
        return transform;
    }
    public PlayerSaveData GetPlayerSaveData()
    {
        return new PlayerSaveData(currentHealth, currentStamina, transform.localPosition, transform.localRotation);
    }

    public void LoadPlayerSaveData(PlayerSaveData saveData)
    {
        currentHealth = saveData.currentHealth;
        currentStamina = saveData.currentStamina;
        OnHealthChanged?.Invoke(currentHealth);
        OnStaminaChanged?.Invoke(currentStamina);
        transform.SetLocalPositionAndRotation(new Vector3(saveData.playerPosition.x, saveData.playerPosition.y + 1, saveData.playerPosition.z), new Quaternion(saveData.playerRotation.x, saveData.playerRotation.y, saveData.playerRotation.z, saveData.playerRotation.w));
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public float currentHealth = 100f;
    public float currentStamina = 100f;
    public SerializableVector3 playerPosition;
    public SerializableQuaternion playerRotation;

    public PlayerSaveData() { }

    public PlayerSaveData(float health, float stamina, Vector3 position, Quaternion rotation)
    {
        currentHealth = health;
        currentStamina = stamina;
        playerPosition = position;
        playerRotation = rotation;
    }
}