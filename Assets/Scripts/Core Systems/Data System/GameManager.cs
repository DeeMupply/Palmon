using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central coordinator for game systems and save/load operations
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Data")]
    [SerializeField] private GameData activeGameData;

    // Other systems
    [Header("Other Systems")]
    public bool IsGameJustLaunched {get; private set;} = true;
    public void SetIsGameJustLaunchedFalse()
    {
        IsGameJustLaunched = false;
    }

    // Events
    // public System.Action<int> OnGameDayChanged;

    #region Unity Lifecycle

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadOrCreateGameData();
    }

    #endregion

    #region Load Data
    
    private void LoadOrCreateGameData()
    {
        // Try to load from save file first
        var loadedData = SaveLoadSystem.LoadGameFromFile(this);
        if (loadedData != null)
        {
            activeGameData = loadedData;

            // Load tool data if implemented
            Player.Instance.LoadToolData(activeGameData.ToolData);
            Player.Instance.LoadPlayerSaveData(activeGameData.PlayerData);
        }
        else
        {
            // Create new game data
            activeGameData = new GameData(this);
            // Initialize other systems as needed
        }
    }

    #endregion

    #region Save Data

    private void SaveGameData()
    {
        if (activeGameData != null)
        {
            // Update tool data
            var toolSaveData = Player.Instance.GetToolSaveData();
            if (toolSaveData == null)
            {
                throw new System.Exception("Player returned null ToolSaveData during save operation.");
            }
            activeGameData.SetToolData(toolSaveData, this);

            // Update player data
            var playerSaveData = Player.Instance.GetPlayerSaveData();
            if (playerSaveData == null)
            {
                throw new System.Exception("Player returned null PlayerSaveData during save operation.");
            }
            activeGameData.SetPlayerData(playerSaveData, this);

            // ✅ SAVE TO FILE
            SaveLoadSystem.SaveGameToFile(this, activeGameData);
        }
    }

    #endregion

    #region Auto-Save Events

    private void OnApplicationPause(bool pauseStatus)
    {
        // Only auto-save if we have valid game data AND game is actually running
        if (pauseStatus && activeGameData != null && Time.time > 5f) // 5 seconds after start
        {
            SaveGameData();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Only auto-save if losing focus AND game has been running for a while
        if (!hasFocus && activeGameData != null && Time.time > 5f) // 5 seconds after start
        {
            SaveGameData();
        }
    }

    private void OnApplicationQuit()
    {
        if (activeGameData != null)
        {
            SaveGameData();
        }
    }
    
    private void OnDestroy()
    {
        // Safety save if GameManager is being destroyed unexpectedly
        if (activeGameData != null && Instance == this)
        {
            SaveGameData();
        }
    }

    #endregion

    [ContextMenu("Debug: Clear Save Data")]
    private void ClearSaveData()
    {
        SaveLoadSystem.DeleteSaveFile(this);
        Debug.Log("Save data cleared.");
    }
}