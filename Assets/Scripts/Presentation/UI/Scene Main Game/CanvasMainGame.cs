using System.Text.RegularExpressions;
using UnityEngine;

public class CanvasMainGame : MonoBehaviour
{
    public static CanvasMainGame Instance { get; private set; }
    [SerializeField] GroupPaused groupPaused;
    [SerializeField] CanvasGroup groupToolsUpgrade;
    [SerializeField] CanvasGroup groupSpecies;

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
        GlobalUIController.HideGroup(groupToolsUpgrade);
        GlobalUIController.HideGroup(groupSpecies);
    }

    public void ShowPausedMenu()
    {
        groupPaused.ShowGroupPaused();
    }

    public void ShowToolsUpgradeMenu()
    {
        GlobalUIController.ShowGroup(groupToolsUpgrade);
    }
    public void HideToolsUpgradeMenu()
    {
        GlobalUIController.HideGroup(groupToolsUpgrade);
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
    private void TogglePausedMenu()
    {
        groupPaused.ToggleGroupPaused();
    }

    [ContextMenu("Toggle Tools Upgrade Menu")]
    private void ToggleToolsUpgradeMenu()
    {
        GlobalUIController.ToggleGroup(groupToolsUpgrade);
    }

    [ContextMenu("Toggle Species Menu")]
    private void ToggleSpeciesMenu()
    {
        GlobalUIController.ToggleGroup(groupSpecies);
    }
}