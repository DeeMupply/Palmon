using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelTool : MonoBehaviour
{
    [SerializeField] private Image toolIcon;
    [SerializeField] private Image toolCooldownOverlay;
    [SerializeField] private Slider toolCooldownSlider;
    [SerializeField] private TextMeshProUGUI toolUsesText;
    [SerializeField] private TextMeshProUGUI toolCooldownText;
    [SerializeField] private ToolSO toolSO;
    private Tool tool;

    private void OnEnable()
    {
        tool = Player.Instance.GetToolByID(toolSO.ID);
        if (tool != null)
        {
            tool.OnToolUsed += OnToolIngameDataChanged;
            tool.OnToolLeveledUp += OnToolIngameDataChanged;
            tool.OnToolCooldownUpdated += OnToolIngameDataChanged;
        }
    }

    private void OnDisable()
    {
        if (tool != null)
        {
            tool.OnToolUsed -= OnToolIngameDataChanged;
            tool.OnToolLeveledUp -= OnToolIngameDataChanged;
            tool.OnToolCooldownUpdated -= OnToolIngameDataChanged;
        }
    }

    private void Start()
    {
        InitToolPanel();
        UpdateToolPanel();
    }

    private void InitToolPanel()
    {
        toolIcon.sprite = toolSO.ToolIcon;
        toolCooldownOverlay.sprite = toolSO.ToolIcon;
        toolCooldownSlider.maxValue = toolSO.CooldownTime;
    }

    private void UpdateToolPanel()
    {
        if (tool != null)
        {
            toolUsesText.text = $"{tool.CurrentLeftUses}/{tool.CurrentMaxUses}";
            toolCooldownText.text = tool.IsOnCooldown ? $"{tool.CurrentCooldown:F1}s" : "Ready";
            toolCooldownSlider.value = tool.CurrentCooldown;
        }
    }

    private void OnToolIngameDataChanged()
    {
        UpdateToolPanel();
    }
}