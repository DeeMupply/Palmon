using UnityEngine;
using UnityEngine.SceneManagement;

public class GroupPaused : MonoBehaviour
{
    [SerializeField] private CanvasGroup groupPaused;
    public void OnResumeButtonPress()
    {
        HideGroupPaused();
        Player.Instance.TogglePauseState();
    }

    public void OnUpgradeButtonPress()
    {
        CanvasMainGame.Instance.ShowToolsUpgradeMenu();
    }

    public void OnSettingsButtonPress()
    {
        CanvasAllScenes.Instance.ShowSettingsGroup();
    }
    
    public void OnRespawnButtonPress()
    {
        Time.timeScale = 1f;
        Player.Instance.Respawn();
        HideGroupPaused();
    }

    public void OnQuitButtonPress()
    {
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

    private void HideGroupPaused()
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