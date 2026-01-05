using UnityEngine;
using UnityEngine.UI;

public class ScannedPopup : MonoBehaviour
{
    [SerializeField] private Image scannedIcon;
    [SerializeField] private TMPro.TextMeshProUGUI scannedNameText;
    [SerializeField] private RectTransform popupTransformPosition;
    [SerializeField] private string scannedPrefix = "Successfully scanned ";
    [SerializeField] private float moveSpeed = 50f; // Units per second to move up
    [SerializeField] private float duration = 1f;
    [SerializeField] private ScannableDatabase scannableDatabase;

    private CanvasGroup canvasGroup;
    private float elapsedTime = 0f;

    private void Awake()
    {
        // Add CanvasGroup if it doesn't exist
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Initialize(string scannableID)
    {
        // Get the scannable data from database
        if (scannableDatabase != null)
        {
            ScannableSO scannableData = scannableDatabase.GetScannableByID(scannableID);
            
            if (scannableData != null)
            {
                // Set the icon
                if (scannedIcon != null)
                {
                    scannedIcon.sprite = scannableData.ScannableIcon;
                }
                
                // Set the text
                if (scannedNameText != null)
                {
                    scannedNameText.text = scannedPrefix + scannableData.ScannableName;
                }
            }
            else
            {
                Debug.LogWarning($"Scannable with ID '{scannableID}' not found in database!");
                if (scannedNameText != null)
                {
                    scannedNameText.text = scannedPrefix + scannableID;
                }
            }
        }
        else
        {
            Debug.LogWarning("ScannableDatabase is not assigned to ScannedPopup!");
            if (scannedNameText != null)
            {
                scannedNameText.text = scannedPrefix + scannableID;
            }
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        
        // Move up
        if (popupTransformPosition != null)
        {
            popupTransformPosition.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
        
        // Fade out
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f - (elapsedTime / duration);
        }
        
        // Destroy after duration
        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
        }
    }
}