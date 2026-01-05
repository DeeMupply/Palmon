using UnityEngine;

public class GroupSettings : MonoBehaviour
{
    [SerializeField] private CanvasGroup settingsGroup;

    private System.Action onCloseComplete;

    public void HideOnStart()
    {
        GlobalUIController.HideGroup(settingsGroup);
    }

    public void ShowWithEffect(System.Action onCloseComplete = null)
    {
        this.onCloseComplete = onCloseComplete;
        GlobalUIController.ShowGroup(settingsGroup, 0.3f);
    }

    public void Toggle()
    {
        GlobalUIController.ToggleGroup(settingsGroup);
    }

    public void OnExitButtonClicked()
    {
        GlobalUIController.HideGroup(settingsGroup, 0.3f);
        SoundManager.Instance.PlayButton();
        onCloseComplete?.Invoke();
    }
    public void OnSaveButtonClicked()
    {
        // Implement saving settings logic here
        SoundManager.Instance.PlayButton();
        GlobalUIController.HideGroup(settingsGroup, 0.3f);
        onCloseComplete?.Invoke();
    }
}