using UnityEngine;
using UnityEngine.UI;

public class GroupStatus : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text healthText;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Text staminaText;

    private void OnEnable()
    {
        Player.Instance.OnHealthChanged += OnHealthChanged;
        Player.Instance.OnStaminaChanged += OnStaminaChanged;
    }

    private void OnDisable()
    {
        Player.Instance.OnHealthChanged -= OnHealthChanged;
        Player.Instance.OnStaminaChanged -= OnStaminaChanged;
    }

    private void Start()
    {
        healthBar.maxValue = Player.Instance.GetMaxHealth();
        staminaBar.maxValue = Player.Instance.GetMaxStamina();
    }

    private void OnHealthChanged(float newHealth)
    {
        healthBar.value = newHealth;
        healthText.text = $"{newHealth}/{Player.Instance.GetMaxHealth()}";
    }

    private void OnStaminaChanged(float newStamina)
    {
        staminaBar.value = newStamina;
        staminaText.text = $"{newStamina}/{Player.Instance.GetMaxStamina()}";
    }
}