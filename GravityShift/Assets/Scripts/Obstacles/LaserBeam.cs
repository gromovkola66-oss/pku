using UnityEngine;

/// <summary>
/// Лазерный луч.
/// Включается/выключается по таймеру.
/// Использует LineRenderer для визуала.
/// Наносит урон при пересечении.
/// </summary>
public class LaserBeam : MonoBehaviour
{
    [Header("Настройки лазера")]
    [Tooltip("Начальная точка (локальная)")]
    [SerializeField] private Vector3 startPoint = Vector3.zero;

    [Tooltip("Конечная точка (локальная)")]
    [SerializeField] private Vector3 endPoint = new Vector3(0, 0, 10);

    [Tooltip("Время включения (сек)")]
    [SerializeField] private float onTime = 2f;

    [Tooltip("Время выключения (сек)")]
    [SerializeField] private float offTime = 2f;

    [Tooltip("Ширина луча")]
    [SerializeField] private float beamWidth = 0.1f;

    [Tooltip("Цвет луча")]
    [SerializeField] private Color beamColor = Color.red;

    // Компоненты
    private LineRenderer lineRenderer;
    private float timer = 0f;
    private bool isActive = true;

    private void Start()
    {
        // Создаём LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;

        // Создаём материал для лазера
        Material laserMat = new Material(Shader.Find("Sprites/Default"));
        laserMat.color = beamColor;
        lineRenderer.material = laserMat;

        UpdateBeamPositions();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float currentCycleTime = isActive ? onTime : offTime;

        if (timer >= currentCycleTime)
        {
            timer = 0f;
            isActive = !isActive;
            lineRenderer.enabled = isActive;
        }

        if (isActive)
        {
            UpdateBeamPositions();
            CheckForPlayer();

            // Мерцание
            float flicker = 0.8f + Mathf.Sin(Time.time * 20f) * 0.2f;
            lineRenderer.startColor = beamColor * flicker;
            lineRenderer.endColor = beamColor * flicker;
        }
    }

    /// <summary>
    /// Обновить позиции луча
    /// </summary>
    private void UpdateBeamPositions()
    {
        lineRenderer.SetPosition(0, transform.TransformPoint(startPoint));
        lineRenderer.SetPosition(1, transform.TransformPoint(endPoint));
    }

    /// <summary>
    /// Проверить пересечение с игроком через рейкаст
    /// </summary>
    private void CheckForPlayer()
    {
        Vector3 worldStart = transform.TransformPoint(startPoint);
        Vector3 worldEnd = transform.TransformPoint(endPoint);
        Vector3 direction = (worldEnd - worldStart).normalized;
        float length = Vector3.Distance(worldStart, worldEnd);

        RaycastHit hit;
        if (Physics.Raycast(worldStart, direction, out hit, length))
        {
            PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }
    }
}
