using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Thiết lập mặt trời")]
    public Light sunLight;
    public float rotationSpeed = 10f;
    public float dayIntensity = 1.2f;
    public float nightIntensity = 0.2f;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.2f, 0.3f, 0.6f);

    [Header("Skybox")]
    public Material stylizedSkyMaterial;
    public Color skyDayTop = new Color(0.17f, 0.57f, 0.69f);
    public Color skyDayBottom = new Color(0.76f, 0.81f, 0.85f);
    public Color skyNightTop = new Color(0.02f, 0.05f, 0.15f);
    public Color skyNightBottom = new Color(0.08f, 0.12f, 0.25f);

    public bool IsNight { get; private set; }

    void Update()
    {
        // Xoay mặt trời
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);

        // Tính góc chiếu
        float dot = Vector3.Dot(transform.forward, Vector3.down);

        // Nội suy cường độ và màu
        sunLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, Mathf.Clamp01(dot));
        sunLight.color = Color.Lerp(nightColor, dayColor, Mathf.Clamp01(dot));

        // Xác định trời tối
        IsNight = sunLight.intensity <= 0.3f;

        // Đổi hiệu ứng skybox ngày/đêm
        if (stylizedSkyMaterial != null)
        {
            stylizedSkyMaterial.SetColor("_SkyGradientTop", Color.Lerp(skyNightTop, skyDayTop, Mathf.Clamp01(dot)));
            stylizedSkyMaterial.SetColor("_SkyGradientBottom", Color.Lerp(skyNightBottom, skyDayBottom, Mathf.Clamp01(dot)));
        }
    }
}
