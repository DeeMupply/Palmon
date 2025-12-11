using System.Text.RegularExpressions;
using UnityEngine;

public class CanvasMainGame : MonoBehaviour
{
    public static CanvasMainGame Instance { get; private set; }
    [SerializeField] GroupPaused groupPaused;
    [SerializeField] GroupToolUpgrade groupToolsUpgrade;
    [SerializeField] CanvasGroup groupSpecies;
    [SerializeField] GroupDeathPopup groupDeathPopup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HideAllMenus();
    }

    private void HideAllMenus()
    {
        groupPaused.HideOnStart();
        groupToolsUpgrade.HideOnStart();
        GlobalUIController.HideGroup(groupSpecies);
    }

    public void ShowPausedMenu()
    {
        groupPaused.ShowGroupPaused();
    }

    public void ShowToolsUpgradeMenu()
    {
        groupToolsUpgrade.ShowWithEffect();
    }
    public void HideToolsUpgradeMenu()
    {
        groupToolsUpgrade.OnExitButtonClicked();
    }
    public void ShowSpeciesMenu()
    {
        GlobalUIController.ShowGroup(groupSpecies);
    }
    public void HideSpeciesMenu()
    {
        GlobalUIController.HideGroup(groupSpecies);
    }

    [ContextMenu("Toggle Paused Menu")]
    public void TogglePausedMenu()
    {
        groupPaused.ToggleGroupPaused();
    }

    [ContextMenu("Toggle Tools Upgrade Menu")]
    private void ToggleToolsUpgradeMenu()
    {
        groupToolsUpgrade.OnExitButtonClicked();
    }

    [ContextMenu("Toggle Species Menu")]
    private void ToggleSpeciesMenu()
    {
        GlobalUIController.ToggleGroup(groupSpecies);
    }

    [ContextMenu("Toggle Death Popup")]
    private void ToggleDeathPopup()
    {
        groupDeathPopup.ToggleGroupDeathPopup();
    }
}