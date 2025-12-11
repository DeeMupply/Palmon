using UnityEngine;

public class CanvasAllScenes : MonoBehaviour
{
    public static CanvasAllScenes Instance { get; private set; }

    [SerializeField] private GroupSettings groupSettings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HideAllGroups();
    }

    private void HideAllGroups()
    {
        groupSettings.HideOnStart();
    }

    public void ShowSettingsGroup(System.Action onCloseComplete = null)
    {
        groupSettings.ShowWithEffect(onCloseComplete);
    }

    [ContextMenu("Toggle Settings Group")]
    public void ToggleSettingsGroup()
    {
        groupSettings.Toggle();
    }
}