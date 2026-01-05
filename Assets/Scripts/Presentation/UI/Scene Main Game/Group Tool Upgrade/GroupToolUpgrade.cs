using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GroupToolUpgrade : MonoBehaviour
{
    [SerializeField] private CanvasGroup upgradeGroup;
    [SerializeField] private TextMeshProUGUI totalAdnText;

    private System.Action onCloseComplete;
    
    private void OnEnable()
    {
        Player.Instance.OnAdnChanged += UpdateTotalAdn;
    }
    private void OnDisable()
    {
        Player.Instance.OnAdnChanged -= UpdateTotalAdn;
    }
    
    public void UpdateTotalAdn(int totalAdn)
    {
        totalAdnText.text = totalAdn.ToString();
    }

    public void HideOnStart()
    {
        GlobalUIController.HideGroup(upgradeGroup);
    }

    public void ShowWithEffect(System.Action onCloseComplete = null)
    {
        this.onCloseComplete = onCloseComplete;
        GlobalUIController.ShowGroup(upgradeGroup);
    }

    public void OnExitButtonClicked()
    {
        SoundManager.Instance.PlayButton();
        GlobalUIController.HideGroup(upgradeGroup);
        onCloseComplete?.Invoke();
    }

    public void OnSwitchTabButtonPressed()
    {
        SoundManager.Instance.PlayButton();
        GlobalUIController.HideGroup(upgradeGroup);
        CanvasMainGame.Instance.ShowSpeciesMenu();
    }
}