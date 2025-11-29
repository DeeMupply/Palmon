using UnityEngine;

public partial class Player : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    [Header("Player Status")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 100f;

    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float staminaRegenRate = 5f; // Stamina regenerated per second
    [SerializeField] private float sprintStaminaCost = 10f; // Stamina cost per second while sprinting

    private void HandleStamina()
    {
        if (isSprinting && moveInput.sqrMagnitude > 0.01f && stamina > 0)
        {
            stamina -= sprintStaminaCost * Time.deltaTime;
            if (stamina <= 0)
            {
                stamina = 0;
                isSprinting = false; // Stop sprinting if out of stamina
            }
        }
        else
        {
            stamina = Mathf.Min(maxStamina, stamina + staminaRegenRate * Time.deltaTime);
        }
    }
}