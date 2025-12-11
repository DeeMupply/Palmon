using UnityEngine;
using UnityEngine.SceneManagement;

public class GroupDeathPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup groupDeathPopup;

    private void OnEnable()
    {
        Player.Instance.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        Player.Instance.OnPlayerDeath -= OnPlayerDeath;
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
        Player.Instance.TogglePauseState();
    }

    private void HideGroupDeathPopup()
    {
        GlobalUIController.HideGroup(groupDeathPopup);
        Player.Instance.TogglePauseState();
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

    public void OnPlayerDeath()
    {
        ShowGroupDeathPopup();
    }
}