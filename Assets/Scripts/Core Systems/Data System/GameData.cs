using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Core game progression data (non-shop related)
/// ShopManager handles all economy and unlock data separately
/// </summary>
[System.Serializable]
public class GameData
{
    [Header("Tool Data")]
    public ToolSaveData ToolData {get; private set;}

    [Header("Player Stats")]
    public PlayerSaveData PlayerData {get; private set;}

    /// <summary>
    /// Default constructor - creates GameData with safe initial values
    /// </summary>
    public GameData(object editor)
    {
        if (!ValidateEditor(editor))
        {
            throw new System.UnauthorizedAccessException("Only GameManager can create GameData instances.");
        }
        ToolData = new ToolSaveData();
        PlayerData = new PlayerSaveData();
    }

    /// <summary>
    /// Validate that only GameManager can edit this data
    /// </summary>
    private bool ValidateEditor(object editor)
    {
        if (editor is GameManager)
        {
            return true;
        }

        return false;
    }

    // Tool data management
    internal bool SetToolData(ToolSaveData data, GameManager editor)
    {
        if (!ValidateEditor(editor))
        {
            throw new System.UnauthorizedAccessException("Only GameManager can modify tool data.");
        }
        ToolData = data;
        return true;
    }

    // Player data management
    internal bool SetPlayerData(PlayerSaveData data, GameManager editor)
    {
        if (!ValidateEditor(editor))
        {
            throw new System.UnauthorizedAccessException("Only GameManager can modify player data.");
        }
        PlayerData = data;
        return true;
    }
}