using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    // Tool-related properties and methods can be added here in the future
    private int currentADN;
    [SerializeField] private List<ToolSO> tools;

    public Dictionary<string, Tool> ToolDictionary {get; private set;}= new Dictionary<string, Tool>();

    public event System.Action OnCurrentToolChanged;

    // Internal fields
    private string currentToolID;

    private void InitializeTools()
    {
        foreach (var toolSO in tools)
        {
            Tool tool = new Tool(toolSO, () => {
                switch (toolSO.ToolType)
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
            ToolDictionary.Add(toolSO.ID, tool);
        }
        currentToolID = tools[0].ID; // Set default tool
    }

    private void UpdateToolCooldowns()
    {
        foreach (var tool in ToolDictionary.Values)
        {
            if (tool.CurrentCooldown > 0)
            {
                tool.CurrentCooldown -= Time.deltaTime;
                if (tool.CurrentCooldown < 0)
                {
                    tool.CurrentCooldown = 0;
                }
                tool.OnToolCooldownUpdated?.Invoke();
            }
        }
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

    public Tool GetCurrentTool()
    {
        return GetToolByID(currentToolID);
    }

    public Tool GetToolByID(string toolID)
    {
        if (ToolDictionary.TryGetValue(toolID, out Tool tool))
        {
            return tool;
        }
        Debug.LogWarning($"Tool with ID {toolID} not found.");
        return null;
    }

    public ToolSaveData GetToolSaveData()
    {
        ToolSaveData saveData = new ToolSaveData();
        saveData.CurrentADN = currentADN;
        saveData.ToolEntries = new List<ToolSaveDataEntry>();

        foreach (var tool in ToolDictionary.Values)
        {
            ToolSaveDataEntry entry = new ToolSaveDataEntry
            {
                ToolID = tool.ToolData.ID,
                CurrentLevel = tool.CurrentLevel
            };
            saveData.ToolEntries.Add(entry);
        }

        return saveData;
    }

    public void LoadToolData(ToolSaveData saveData)
    {
        currentADN = saveData.CurrentADN;

        foreach (var entry in saveData.ToolEntries)
        {
            Tool tool = GetToolByID(entry.ToolID);
            if (tool != null)
            {
                tool.CurrentLevel = entry.CurrentLevel;
                tool.ResetUses();
            }
        }
    }
}

public class ToolSaveData
{
    public int CurrentADN;
    public List<ToolSaveDataEntry> ToolEntries;
}

public class ToolSaveDataEntry
{
    public string ToolID;
    public int CurrentLevel;
}

public class Tool
{
    public ToolSO ToolData;
    public int CurrentLevel = 1;
    public int CurrentMaxUses => ToolData.GetMaxUsesAtLevel(CurrentLevel);
    public int CurrentLeftUses;
    public float CurrentCooldown;
    public bool IsOnCooldown => CurrentCooldown > 0;

    public System.Action OnToolUsed;
    public System.Action OnToolLeveledUp;
    public System.Action OnToolCooldownUpdated;

    public Tool(ToolSO toolSO, System.Action onToolUsed)
    {
        ToolData = toolSO;
        OnToolUsed = onToolUsed;
        CurrentLeftUses = CurrentMaxUses;
        CurrentCooldown = toolSO.CooldownTime;
    }

    public void ResetUses()
    {
        CurrentLeftUses = CurrentMaxUses;
    }

    public void UseTool()
    {
        if (CurrentLeftUses > 0)
        {
            CurrentLeftUses--;
            OnToolUsed?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Tool {ToolData.ToolName} has reached its maximum uses.");
        }
    }

    public void LevelUp()
    {
        if (CurrentLevel < ToolData.MaxLevel)
        {
            CurrentLevel++;
            ResetUses();
            // Implement additional level-up logic here
        }
        else
        {
            Debug.LogWarning($"Tool {ToolData.ToolName} is already at maximum level.");
        }
    }

    public int GetADNRequirement()
    {
        return ToolData.AdnPerLevel * CurrentLevel;
    }
}
