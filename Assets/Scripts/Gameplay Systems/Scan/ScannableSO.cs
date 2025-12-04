using UnityEngine;

[CreateAssetMenu(fileName = "New Scannable", menuName = "Gameplay/Scannable")]
public class ScannableSO : ScriptableObject
{
    [Header("Scannable Data")]
    [SerializeField] private string scannableName;
    [SerializeField] private string scannableDescription;
    [SerializeField] private Sprite scannableIcon;

    [Header("Gameplay")]
    [SerializeField] private int scansRequired = 20;
    [SerializeField] private float scanTime = 2.0f;
    [SerializeField] private int adnPerScan = 10;

    public string ScannableName => scannableName;
    public string ScannableDescription => scannableDescription;
    public Sprite ScannableIcon => scannableIcon;
    public int ScansRequired => scansRequired;
    public float ScanTime => scanTime;
    public int AdnPerScan => adnPerScan;
    public string ID => name.Replace(" ", "");

    private void OnValidate()
    {
        scannableName = name;
    }
}