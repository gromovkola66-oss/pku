using UnityEngine;

/// <summary>
/// Появляющаяся/исчезающая платформа.
/// Мигает по таймеру: то видна и твёрдая, то исчезает.
/// </summary>
public class AppearingPlatform : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Время видимости (сек)")]
    [SerializeField] private float visibleTime = 2f;

    [Tooltip("Время невидимости (сек)")]
    [SerializeField] private float invisibleTime = 2f;

    [Tooltip("Начальная задержка (сек) — для создания паттернов")]
    [SerializeField] private float startDelay = 0f;

    // Компоненты
    private Renderer platformRenderer;
    private Collider platformCollider;
    private float timer;
    private bool isVisible = true;
    private Color originalColor;

    private void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider>();
        timer = -startDelay; // Отрицательный для задержки
        if (platformRenderer != null)
        {
            originalColor = platformRenderer.material.color;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < 0f) return; // Ещё в задержке

        float cycleTime = isVisible ? visibleTime : invisibleTime;

        // Предупреждение: мигаем за 0.5 сек до исчезновения
        if (isVisible && timer > cycleTime - 0.5f && platformRenderer != null)
        {
            float blink = Mathf.Sin(Time.time * 20f) > 0 ? 1f : 0.3f;
            Color c = originalColor;
            c.a = blink;
            platformRenderer.material.color = c;
        }

        if (timer >= cycleTime)
        {
            timer = 0f;
            isVisible = !isVisible;

            // Включаем/выключаем платформу
            if (platformRenderer != null)
            {
                platformRenderer.enabled = isVisible;
                if (isVisible) platformRenderer.material.color = originalColor;
            }
            if (platformCollider != null)
            {
                platformCollider.enabled = isVisible;
            }
        }
    }
}
