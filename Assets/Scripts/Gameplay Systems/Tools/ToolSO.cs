using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Gameplay/Tool")]
public class ToolSO : ScriptableObject
{
    public ToolType toolType;
    public string ID;
    public string toolName;
    public Sprite toolIcon;
    public int baseMaxUses;
    public int useIncrementPerLevel;
    public int adnPerLevel;
    public int maxLevel;
    public float cooldownTime;

    private void OnValidate()
    {
        ID = toolType.ToString();
        toolName = name;
    }

    public int GetMaxUsesAtLevel(int level)
    {
        return baseMaxUses + (useIncrementPerLevel * (level - 1));
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