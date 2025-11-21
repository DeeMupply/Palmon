using UnityEngine;
using UnityEngine.SceneManagement;

public class GroupMainButtons : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainButtonsGroup;
    [SerializeField] private CanvasGroup settingsGroup;
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNameManager.SCENE_GAMEPLAY);
    }

    public void OnSettingsButtonClicked()
    {
        GlobalUIController.HideGroup(mainButtonsGroup, 0.3f);
        GlobalUIController.ShowGroup(settingsGroup, 0.3f);
    }

    public void OnQuitButtonClicked()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
