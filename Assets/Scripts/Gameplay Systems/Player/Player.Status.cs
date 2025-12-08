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
    [SerializeField] private float sprintStaminaCost = 10f; // Stamina cost per second while sprinting

    public Action<float> OnHealthChanged;
    public Action<float> OnStaminaChanged;

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
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
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
    private void Die()
    {
        Debug.Log("Player has died.");
        // Additional death handling logic can be added here
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