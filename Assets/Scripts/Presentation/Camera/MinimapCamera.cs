using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MiniMapCameraScript : MonoBehaviour
{
    public Transform player;

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
}
