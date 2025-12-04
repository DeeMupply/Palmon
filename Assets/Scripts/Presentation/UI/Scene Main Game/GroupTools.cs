using UnityEngine;
using UnityEngine.UI;

public class GroupTools : MonoBehaviour
{
    [SerializeField] private Image currentToolIcon;
    private void OnEnable()
    {
        Player.Instance.OnCurrentToolChanged += OnCurrentToolChanged;
    }

    private void OnDisable()
    {
        Player.Instance.OnCurrentToolChanged -= OnCurrentToolChanged;
    }

    private void OnCurrentToolChanged()
    {
        Tool currentTool = Player.Instance.GetCurrentTool();
        if (currentTool != null)
        {
            currentToolIcon.sprite = currentTool.ToolData.ToolIcon;
        }
    }
}