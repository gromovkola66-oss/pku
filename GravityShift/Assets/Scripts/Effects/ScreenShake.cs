using UnityEngine;

/// <summary>
/// Тряска экрана.
/// Используется при приземлении и получении урона.
/// </summary>
public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private Transform cameraTransform;
    private Vector3 originalLocalPos;
    private float shakeTimer = 0f;
    private float shakeIntensity = 0f;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Привязать камеру
    /// </summary>
    public void SetCamera(Transform cam)
    {
        cameraTransform = cam;
        if (cameraTransform != null)
            originalLocalPos = cameraTransform.localPosition;
    }

    /// <summary>
    /// Запустить тряску
    /// </summary>
    public void Shake(float intensity = 0.2f, float duration = 0.3f)
    {
        shakeIntensity = intensity;
        shakeTimer = duration;
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.unscaledDeltaTime;

            float currentIntensity = shakeIntensity * (shakeTimer / 0.3f);
            Vector3 offset = new Vector3(
                Random.Range(-currentIntensity, currentIntensity),
                Random.Range(-currentIntensity, currentIntensity),
                0f
            );

            cameraTransform.localPosition = originalLocalPos + offset;
        }
        else
        {
            cameraTransform.localPosition = originalLocalPos;
        }
    }
}
