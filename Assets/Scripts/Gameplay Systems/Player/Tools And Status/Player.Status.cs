using UnityEngine;
using System;

public partial class Player
{
    [Header("Player Status")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaRegenRate = 5f; // Stamina regenerated per second
    [SerializeField] private float staminaRegenRateWhileIdle = 7f; // Stamina regenerated per second while idle
    [SerializeField] private float sprintStaminaCost = 10f; // Stamina cost per second while sprinting

    [SerializeField] Player_OnEvent_Behaviour onDieBehaviour;

    public Action<float> OnHealthChanged;
    public Action<float> OnStaminaChanged;
    public Action OnPlayerDeath;

    private void InitializeStatus()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        OnHealthChanged?.Invoke(currentHealth);
        OnStaminaChanged?.Invoke(currentStamina);
    }

    private void HandleStamina()
    {
        if (isSprinting && moveInput.sqrMagnitude > 0.01f && currentStamina > 0)
        {
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false; // Stop sprinting if out of stamina
            }
            OnStaminaChanged?.Invoke(currentStamina);
        }
        else
        {
            float regenRate = (!isMoving) ? staminaRegenRateWhileIdle : staminaRegenRate;
            currentStamina = Mathf.Min(maxStamina, currentStamina + regenRate * Time.deltaTime);
            OnStaminaChanged?.Invoke(currentStamina);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void RegenHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth);
    }
    
    private void Die()
    {
        Debug.Log("Player has died.");
        // Additional death handling logic can be added here
        isDying = true;
    }

    public void InvokeDeathEvent()
    {
        OnPlayerDeath?.Invoke();
    }

    public Player_OnEvent_Behaviour GetOnDieBehaviour()
    {
        return onDieBehaviour;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }

    [ContextMenu("Remove 10% Health")]
    private void RemoveTenPercentHealth()
    {
        TakeDamage(maxHealth * 0.1f);
    }
}