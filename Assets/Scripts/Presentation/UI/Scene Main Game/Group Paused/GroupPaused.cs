using UnityEngine;
using UnityEngine.SceneManagement;

public class GroupPaused : MonoBehaviour
{
    [SerializeField] private CanvasGroup groupPaused;
    public void OnResumeButtonPress()
    {
        SoundManager.Instance.PlayButton();
        HideGroupPaused();
        Player.Instance.SetPauseState(false);
    }

    public void OnUpgradeButtonPress()
    {
        SoundManager.Instance.PlayButton();
        CanvasMainGame.Instance.ShowToolsUpgradeMenu();
    }

    public void OnSpeciesButtonPress()
    {
        SoundManager.Instance.PlayButton();
        CanvasMainGame.Instance.ShowSpeciesMenu();
    }
    
    public void OnSettingsButtonPress()
    {
        SoundManager.Instance.PlayButton();
        CanvasAllScenes.Instance.ShowSettingsGroup();
    }
    
    public void OnRespawnButtonPress()
    {
        SoundManager.Instance.PlayButton();
        Time.timeScale = 1f;
        Player.Instance.Respawn();
        HideGroupPaused();
    }

    public void OnQuitButtonPress()
    {
        SoundManager.Instance.PlayButton();
        Time.timeScale = 1f;
        // TODO: Implement player data saving logic here
        GameManager.Instance.SaveGameData();
        SceneManager.LoadScene(SceneNameManager.SCENE_MAIN_MENU);
    }

    public void HideOnStart()
    {
        GlobalUIController.HideGroup(groupPaused);
    }

    public void ShowGroupPaused()
    {
        Time.timeScale = 0f;
        GlobalUIController.ShowGroup(groupPaused);
    }

    public void HideGroupPaused()
    {
        Time.timeScale = 1f;
        GlobalUIController.HideGroup(groupPaused);
    }

    public void ToggleGroupPaused()
    {
        if (groupPaused.alpha == 0f)
        {
            ShowGroupPaused();
        }
        else
        {
            HideGroupPaused();
        }
    }
}