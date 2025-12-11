using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    // Tool-related properties and methods can be added here in the future
    private int currentADN;
    [SerializeField] private List<ToolSO> tools;

    public Dictionary<string, Tool> ToolDictionary {get; private set;}= new Dictionary<string, Tool>();

    [SerializeField] private List<IngameToolObjectReferences> ingameToolObjectReferences;

    private Dictionary<string, IngameToolObjectReferences> ingameToolObjectReferenceDictionary = new Dictionary<string, IngameToolObjectReferences>();

    public event System.Action OnCurrentToolChanged;
    public event System.Action<int> OnAdnChanged;

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
    }

    private void InitializeIngameToolObjectReferences()
    {
        foreach (var reference in ingameToolObjectReferences)
        {
            reference.ToolObjectInHand.SetActive(false);
            reference.ToolObjectInPocket.SetActive(false);
            ingameToolObjectReferenceDictionary[reference.ToolData.ID] = reference;
        }
    }

    private void InitializeCurrentTool()
    {
        SwitchToTool(tools[0].ID); // Default to first tool
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

    private bool SwitchToTool(string toolID)
    {
        if (ToolDictionary.ContainsKey(toolID))
        {
            if (currentToolID != null && !string.IsNullOrEmpty(currentToolID))
                ingameToolObjectReferenceDictionary[ currentToolID ].ToolObjectInPocket.SetActive(false);
            currentToolID = toolID;
            ingameToolObjectReferenceDictionary[ currentToolID ].ToolObjectInPocket.SetActive(true);
            OnCurrentToolChanged?.Invoke();
            return true;
        }
        Debug.LogWarning($"Tool with ID {toolID} not found.");
        return false;
    }

    public void TakeoutTool()
    {
        ingameToolObjectReferenceDictionary[ currentToolID ].ToolObjectInPocket.SetActive(false);
        ingameToolObjectReferenceDictionary[ currentToolID ].ToolObjectInHand.SetActive(true);
    }

    public void UseTool()
    {
        ToolDictionary[currentToolID].UseTool();
    }

    public void ReturnTool()
    {
        ingameToolObjectReferenceDictionary[ currentToolID ].ToolObjectInHand.SetActive(false);
        ingameToolObjectReferenceDictionary[ currentToolID ].ToolObjectInPocket.SetActive(true);
        isUsingTool = false;
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
        ToolSaveData saveData = new ToolSaveData(currentToolID, currentADN);

        foreach (var tool in ToolDictionary.Values)
        {
            ToolSaveDataEntry entry = new ToolSaveDataEntry
            {
                ToolID = tool.ToolData.ID,
                CurrentLevel = tool.CurrentLevel,
                CurrentCooldown = tool.CurrentCooldown,
                CurrentLeftUses = tool.CurrentLeftUses
            };
            saveData.ToolEntries.Add(entry);
        }

        return saveData;
    }

    public void LoadToolData(ToolSaveData saveData)
    {
        if (saveData == null) return;
        currentADN = saveData.CurrentADN;
        currentToolID = saveData.CurrentToolID;
        if (saveData.ToolEntries == null  || saveData.ToolEntries.Count == 0) return;
        foreach (var entry in saveData.ToolEntries)
        {
            Tool tool = GetToolByID(entry.ToolID);
            if (tool != null)
            {
                tool.CurrentLevel = entry.CurrentLevel;
                tool.CurrentCooldown = entry.CurrentCooldown;
                tool.CurrentLeftUses = entry.CurrentLeftUses;
            }
        }
        SwitchToTool(currentToolID);
    }

    private void ReplenishAllTools()
    {
        foreach (var tool in ToolDictionary.Values)
        {
            tool.ResetUses();
            tool.CurrentCooldown = 0f;
            tool.OnToolReplenished?.Invoke();
        }
    }

    public void AddADN(int amount)
    {
        currentADN += amount;
        OnAdnChanged?.Invoke(currentADN);
    }

    public void DeductADN(int amount)
    {
        currentADN -= amount;
        if (currentADN < 0)
            currentADN = 0;
        OnAdnChanged?.Invoke(currentADN);
    }

    [System.Serializable]
    public class IngameToolObjectReferences
    {
        public ToolSO ToolData;
        public GameObject ToolObjectInPocket;
        public GameObject ToolObjectInHand;
    }
}

[System.Serializable]
public class ToolSaveData
{
    public int CurrentADN;
    public string CurrentToolID;
    public List<ToolSaveDataEntry> ToolEntries;
    public ToolSaveData()
    {
        ToolEntries = new List<ToolSaveDataEntry>();
    }
    public ToolSaveData(string currentToolID, int currentADN)
    {
        CurrentToolID = currentToolID;
        CurrentADN = currentADN;
        ToolEntries = new List<ToolSaveDataEntry>();
    }
}

[System.Serializable]
public class ToolSaveDataEntry
{
    public string ToolID;
    public int CurrentLevel;
    public float CurrentCooldown;
    public int CurrentLeftUses;
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
    public System.Action OnToolReplenished;

    public Tool(ToolSO toolSO, System.Action onToolUsed)
    {
        ToolData = toolSO;
        OnToolUsed = onToolUsed;
        CurrentLeftUses = CurrentMaxUses;
        CurrentCooldown = 0f;
    }

    public void ResetUses()
    {
        CurrentLeftUses = CurrentMaxUses;
    }

    public void UseTool()
    {
        if (ToolData.ToolType == ToolType.Scan)
        {
            OnToolUsed?.Invoke();
            return;
        }

        if (IsOnCooldown)
        {
            Debug.LogWarning($"Tool {ToolData.ToolName} is on cooldown for {CurrentCooldown:F1} more seconds.");
            return;
        }

        if (CurrentLeftUses > 0)
        {
            CurrentLeftUses--;
            CurrentCooldown = ToolData.CooldownTime;
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
