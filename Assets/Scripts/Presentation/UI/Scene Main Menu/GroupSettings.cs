using UnityEngine;

public class GroupSettings : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainButtonsGroup;
    [SerializeField] private CanvasGroup settingsGroup;

    private void Start()
    {
        GlobalUIController.HideGroup(settingsGroup, 0f);
    }

    public void OnExitButtonClicked()
    {
        GlobalUIController.HideGroup(settingsGroup, 0.3f);
        GlobalUIController.ShowGroup(mainButtonsGroup, 0.3f);
    }
    public void OnSaveButtonClicked()
    {
        // Implement saving settings logic here

        GlobalUIController.HideGroup(settingsGroup, 0.3f);
        GlobalUIController.ShowGroup(mainButtonsGroup, 0.3f);
    }
}