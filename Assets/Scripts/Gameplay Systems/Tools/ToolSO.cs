using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Gameplay/Tool")]
public class ToolSO : ScriptableObject
{
    [Header("General Info")]
    [SerializeField] private ToolType toolType;
    [SerializeField] private string toolName;
    [SerializeField] private Sprite toolIcon;

    [Header("Gameplay Stats")]
    [SerializeField] private int baseMaxUses;
    [SerializeField] private int useIncrementPerLevel;
    [SerializeField] private int adnPerLevel;
    [SerializeField] private int maxLevel;
    [SerializeField] private float cooldownTime;

    public ToolType ToolType => toolType;
    public string ToolName => toolName;
    public Sprite ToolIcon => toolIcon;
    public int BaseMaxUses => baseMaxUses;
    public int UseIncrementPerLevel => useIncrementPerLevel;
    public int AdnPerLevel => adnPerLevel;
    public int MaxLevel => maxLevel;
    public float CooldownTime => cooldownTime;
    
    public string ID => ToolType.ToString();

    private void OnValidate()
    {
        toolName = name;
    }

    public int GetMaxUsesAtLevel(int level)
    {
        return baseMaxUses + (useIncrementPerLevel * (level - 1));
    }

    public int GetUpgradeCostAtLevel(int level)
    {
        return adnPerLevel * level;
    }
}

public enum ToolType
{
    Scan,
    Bait,
    Heal,
    Invisible,
    Detect
}