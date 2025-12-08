using UnityEngine;
using UnityEngine.SceneManagement;

public class GroupMainButtons : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainButtonsGroup;

    public void ShowGroupMainButtons()
    {
        GlobalUIController.ShowGroup(mainButtonsGroup, 0.3f);
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(SceneNameManager.SCENE_GAMEPLAY);
    }

    public void OnSettingsButtonClicked()
    {
        GlobalUIController.HideGroup(mainButtonsGroup, 0.3f);
        CanvasAllScenes.Instance.ShowSettingsGroup(ShowGroupMainButtons);
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
