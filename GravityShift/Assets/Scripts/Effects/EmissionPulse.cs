using UnityEngine;

/// <summary>
/// Пульсация свечения (emission) на материале.
/// Прикрепляется к опасным объектам для визуальной обратной связи.
/// </summary>
public class EmissionPulse : MonoBehaviour
{
    [Header("Настройки пульсации")]
    [Tooltip("Скорость пульсации")]
    [SerializeField] private float pulseSpeed = 3f;

    [Tooltip("Минимальная интенсивность")]
    [SerializeField] private float minIntensity = 0.5f;

    [Tooltip("Максимальная интенсивность")]
    [SerializeField] private float maxIntensity = 3f;

    private Renderer objectRenderer;
    private Color baseEmissionColor;
    private bool hasEmission = false;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null && objectRenderer.material.HasProperty("_EmissionColor"))
        {
            baseEmissionColor = objectRenderer.material.GetColor("_EmissionColor");
            hasEmission = true;
        }
    }

    private void Update()
    {
        if (!hasEmission || objectRenderer == null) return;

        // Синусоидальная пульсация
        float intensity = Mathf.Lerp(minIntensity, maxIntensity,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);

        // Нормализуем цвет вручную (делаем максимальный компонент = 1)
        float maxComponent = Mathf.Max(baseEmissionColor.r, Mathf.Max(baseEmissionColor.g, baseEmissionColor.b));
        Color normalizedColor = maxComponent > 0 ? baseEmissionColor / maxComponent : Color.white;
        Color pulsedColor = normalizedColor * intensity;
        objectRenderer.material.SetColor("_EmissionColor", pulsedColor);
    }
}
