using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image toolIcon;
    [SerializeField] private TextMeshProUGUI toolNameText;
    [SerializeField] private TextMeshProUGUI toolLevelText;
    [SerializeField] private TextMeshProUGUI toolCooldownText;
    [SerializeField] private TextMeshProUGUI toolUsesText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private ToolSO toolSO;
    private Tool tool;

    private void OnEnable()
    {
        tool = Player.Instance.GetToolByID(toolSO.ID);
        if (tool != null)
        {
            tool.OnToolLeveledUp += OnToolIngameDataChanged;
        }
    }

    private void OnDisable()
    {
        if (tool != null)
        {
            tool.OnToolLeveledUp -= OnToolIngameDataChanged;
        }
    }

    private void Start()
    {
        InitToolUpgradeItem();
        UpdateToolUpgradeItem();
    }

    private void InitToolUpgradeItem()
    {
        toolIcon.sprite = toolSO.ToolIcon;
        toolNameText.text = toolSO.ToolName;
    }

    private void UpdateToolUpgradeItem()
    {
        if (tool != null)
        {
            toolLevelText.text = $"Level {tool.CurrentLevel}";
            upgradeCostText.text = $"Cost: {toolSO.GetUpgradeCostAtLevel(tool.CurrentLevel)} ADN";
            toolCooldownText.text = $"Cooldown: {toolSO.CooldownTime} s";
            toolUsesText.text = $"Uses: {toolSO.GetMaxUsesAtLevel(tool.CurrentLevel)}";
        }
    }

    private void OnToolIngameDataChanged()
    {
        UpdateToolUpgradeItem();
    }
}