using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GroupStatus : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider healthBarSlowEffect;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private float slowEffectDuration = 1.5f; // How long the animation takes
    [SerializeField] private float slowEffectDelay = 0.3f; // Delay before animation starts
    [SerializeField] private AnimationCurve slowEffectCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Smooth easing
    
    private float targetHealth;
    private Coroutine slowEffectCoroutine;

    private void OnEnable()
    {
        Player.Instance.OnHealthChanged += OnHealthChanged;
        Player.Instance.OnStaminaChanged += OnStaminaChanged;
    }

    private void OnDisable()
    {
        Player.Instance.OnHealthChanged -= OnStaminaChanged;
        Player.Instance.OnStaminaChanged -= OnStaminaChanged;
    }

    private void Start()
    {
        float maxHealth = Player.Instance.GetMaxHealth();
        float maxStamina = Player.Instance.GetMaxStamina();
        
        healthBar.maxValue = maxHealth;
        healthBarSlowEffect.maxValue = maxHealth;
        staminaBar.maxValue = maxStamina;
        
        // Initialize both health bars to current health
        float currentHealth = Player.Instance.GetCurrentHealth();
        healthBar.value = currentHealth;
        healthBarSlowEffect.value = currentHealth;
        targetHealth = currentHealth;
    }

    private void OnHealthChanged(float newHealth)
    {
        // Update the main health bar immediately
        healthBar.value = newHealth;
        healthText.text = $"{Mathf.Round(newHealth)}";
        
        // Handle slow effect animation
        if (newHealth < targetHealth)
        {
            // Health decreased - start slow effect with delay
            targetHealth = newHealth;
            
            // Stop any existing animation
            if (slowEffectCoroutine != null)
                StopCoroutine(slowEffectCoroutine);
            
            slowEffectCoroutine = StartCoroutine(AnimateSlowEffect());
        }
        else
        {
            // Health increased (healing) - update immediately
            if (slowEffectCoroutine != null)
                StopCoroutine(slowEffectCoroutine);
            
            healthBarSlowEffect.value = newHealth;
            targetHealth = newHealth;
        }
    }

    private IEnumerator AnimateSlowEffect()
    {
        // Wait for the delay
        yield return new WaitForSeconds(slowEffectDelay);
        
        float startValue = healthBarSlowEffect.value;
        float elapsedTime = 0f;
        
        while (elapsedTime < slowEffectDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / slowEffectDuration;
            float easedProgress = slowEffectCurve.Evaluate(progress);
            
            healthBarSlowEffect.value = Mathf.Lerp(startValue, targetHealth, easedProgress);
            
            yield return null;
        }
        
        // Ensure we reach the exact target
        healthBarSlowEffect.value = targetHealth;
        slowEffectCoroutine = null;
    }

    private void OnStaminaChanged(float newStamina)
    {
        staminaBar.value = newStamina;
        staminaText.text = $"{Mathf.Round(newStamina)}";
    }
}