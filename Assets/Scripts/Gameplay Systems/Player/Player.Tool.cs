using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class Player : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    // Tool-related properties and methods can be added here in the future
    [SerializeField] private List<ToolSO> tools;

    private Dictionary<string, Tool> toolDictionary = new Dictionary<string, Tool>();

    // Internal fields
    private string currentToolID;

    private void InitializeTools()
    {
        foreach (var toolSO in tools)
        {
            Tool tool = new Tool(toolSO, () => {
                switch (toolSO.toolType)
                {
                    case ToolType.Scan:
                        UseToolScan();
                        break;
                    case ToolType.Bait:
                        UseToolBait();
                        break;
                    case ToolType.Heal:
                        UseToolHeal();
                        break;
                    case ToolType.Invisible:
                        UseToolInvisible();
                        break;
                    case ToolType.Detect:
                        UseToolDetect();
                        break;
                }
            });
            toolDictionary.Add(toolSO.ID, tool);
        }
        currentToolID = tools[0].ID; // Set default tool
    }

    private void UseToolScan()
    {
        // Implement scan tool usage logic here
    }
    
    private void UseToolBait()
    {
        // Implement bait tool usage logic here
    }

    private void UseToolHeal()
    {
        // Implement heal tool usage logic here
    }

    private void UseToolInvisible()
    {
        // Implement invisible tool usage logic here
    }

    private void UseToolDetect()
    {
        // Implement detect tool usage logic here
    }
}

public class Tool
{
    public ToolSO ToolData;
    public int CurrentLevel = 1;
    public int CurrentMaxUses => ToolData.GetMaxUsesAtLevel(CurrentLevel);
    public int CurrentUses;
    public float CurrentCooldown;

    public System.Action OnToolUsed;

    public Tool(ToolSO toolSO, System.Action onToolUsed = null)
    {
        ToolData = toolSO;
        OnToolUsed = onToolUsed;
    }

    public void UseTool()
    {
        if (CurrentUses < CurrentMaxUses)
        {
            CurrentUses++;
            OnToolUsed?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Tool {ToolData.toolName} has reached its maximum uses.");
        }
    }

    public void LevelUp()
    {
        if (CurrentLevel < ToolData.maxLevel)
        {
            CurrentLevel++;
            // Implement additional level-up logic here
        }
        else
        {
            Debug.LogWarning($"Tool {ToolData.toolName} is already at maximum level.");
        }
    }

    public int GetADNRequirement()
    {
        return ToolData.adnPerLevel * CurrentLevel;
    }
}
