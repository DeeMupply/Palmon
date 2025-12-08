using UnityEngine;
using UnityEngine.InputSystem;

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
            InitializeTools();
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
        InitializeCurrentTool();
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

    public PlayerSaveData GetPlayerSaveData()
    {
        return new PlayerSaveData(currentHealth, currentStamina, currentToolID, transform.position);
    }

    public void LoadPlayerSaveData(PlayerSaveData saveData)
    {
        currentHealth = saveData.currentHealth;
        currentStamina = saveData.currentStamina;
        currentToolID = saveData.currentToolID;
        transform.position = new Vector3(saveData.playerPosition.x, saveData.playerPosition.y + 1, saveData.playerPosition.z);
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public float currentHealth = 100f;
    public float currentStamina = 100f;
    public string currentToolID = "";
    public SerializableVector3 playerPosition;

    public PlayerSaveData() { }

    public PlayerSaveData(float health, float stamina, string toolID, Vector3 position)
    {
        currentHealth = health;
        currentStamina = stamina;
        currentToolID = toolID;
        playerPosition = position;
    }
}