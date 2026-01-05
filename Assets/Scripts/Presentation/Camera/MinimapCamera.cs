using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MiniMapCameraScript : MonoBehaviour
{
    public static MiniMapCameraScript Instance;
    public Transform player;
    [SerializeField] private float cameraHeightNotDetecting = 30f;
    [SerializeField] private float cameraHeightDetecting = 45f;

    void Awake()
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

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y; // giữ nguyên độ cao camera
        transform.position = newPosition;
    }

    void Start()
    {
        var uacd = GetComponent<UniversalAdditionalCameraData>();
        if (uacd != null)
        {
            uacd.renderShadows = false; // camera will not render shadows
        }
        else
        {
            Debug.LogWarning("No UniversalAdditionalCameraData found (are you using URP?)");
        }
    }

    public void SetCameraHeightToDetecting()
    {
        Vector3 position = transform.localPosition;
        position.y = cameraHeightDetecting;
        transform.localPosition = position;
    }

    public void SetCameraHeightToNotDetecting()
    {
        Vector3 position = transform.localPosition;
        position.y = cameraHeightNotDetecting;
        transform.localPosition = position;
    }
}
