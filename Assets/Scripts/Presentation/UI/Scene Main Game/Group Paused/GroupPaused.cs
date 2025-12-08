using UnityEngine;
using UnityEngine.SceneManagement;

public class GroupPaused : MonoBehaviour
{
    [SerializeField] private CanvasGroup groupPaused;
    public void OnResumeButtonPress()
    {
        GlobalUIController.HideGroup(groupPaused);
        Time.timeScale = 1f;
        Player.Instance.OnResumeFromPause();
    }

    public void OnSettingsButtonPress()
    {
        CanvasAllScenes.Instance.ShowSettingsGroup();
    }

    public void OnQuitButtonPress()
    {
        Time.timeScale = 1f;
        // TODO: Implement player data saving logic here
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

    public void ToggleGroupPaused()
    {
        GlobalUIController.ToggleGroup(groupPaused);
    }
}