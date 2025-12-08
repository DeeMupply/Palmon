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

    public void OnExitButtonClicked()
    {
        GlobalUIController.HideGroup(settingsGroup, 0.3f);
        onCloseComplete?.Invoke();
    }
    public void OnSaveButtonClicked()
    {
        // Implement saving settings logic here

        GlobalUIController.HideGroup(settingsGroup, 0.3f);
        onCloseComplete?.Invoke();
    }
}