using UnityEngine;
using UnityEngine.SceneManagement;

public class GroupWinPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup groupDeathPopup;

    private void OnEnable()
    {
        Player.Instance.OnPlayerWin += OnPlayerWin;
    }

    private void OnDisable()
    {
        Player.Instance.OnPlayerWin -= OnPlayerWin;
    }

    private void Start()
    {
        HideOnStart();
    }

    public void OnRespawnButtonPress()
    {
        Player.Instance.Respawn();
        HideGroupDeathPopup();
    }

    public void OnQuitButtonPress()
    {
        // TODO: Implement player data saving logic here
        GameManager.Instance.SaveGameData();
        SceneManager.LoadScene(SceneNameManager.SCENE_MAIN_MENU);
    }

    public void HideOnStart()
    {
        GlobalUIController.HideGroup(groupDeathPopup);
    }

    public void ShowGroupDeathPopup()
    {
        GlobalUIController.ShowGroup(groupDeathPopup);
        Player.Instance.SetPauseState(true);
    }

    private void HideGroupDeathPopup()
    {
        GlobalUIController.HideGroup(groupDeathPopup);
        Player.Instance.SetPauseState(false);
    }

    public void ToggleGroupDeathPopup()
    {
        if (groupDeathPopup.alpha == 0f)
        {
            ShowGroupDeathPopup();
        }
        else
        {
            HideGroupDeathPopup();
        }
    }

    public void OnPlayerWin()
    {
        ShowGroupDeathPopup();
    }
}