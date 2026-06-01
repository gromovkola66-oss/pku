using UnityEngine;

/// <summary>
/// Падающая платформа.
/// Падает через 2 секунды после того как игрок наступил.
/// Трясётся перед падением как предупреждение.
/// Возвращается через 4 секунды.
/// </summary>
public class FallingPlatform : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Задержка перед падением (сек)")]
    [SerializeField] private float fallDelay = 2f;

    [Tooltip("Время до восстановления (сек)")]
    [SerializeField] private float respawnDelay = 4f;

    [Tooltip("Скорость падения")]
    [SerializeField] private float fallSpeed = 15f;

    [Tooltip("Интенсивность тряски")]
    [SerializeField] private float shakeIntensity = 0.05f;

    // Состояние
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isTriggered = false;
    private bool isFalling = false;
    private float triggerTimer = 0f;
    private float respawnTimer = 0f;
    private Renderer platformRenderer;
    private Color originalColor;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Платформа не может быть статичной — она двигается
        gameObject.isStatic = false;

        platformRenderer = GetComponent<Renderer>();
        if (platformRenderer != null)
        {
            originalColor = platformRenderer.material.color;
        }
    }

    private void Update()
    {
        if (isTriggered && !isFalling)
        {
            triggerTimer += Time.deltaTime;

            // Тряска как предупреждение
            float shakeX = Random.Range(-shakeIntensity, shakeIntensity);
            float shakeZ = Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = originalPosition + new Vector3(shakeX, 0, shakeZ);

            // Меняем цвет с жёлтого на красный
            float progress = triggerTimer / fallDelay;
            if (platformRenderer != null)
            {
                platformRenderer.material.color = Color.Lerp(originalColor, Color.red, progress);
            }

            // Время падать
            if (triggerTimer >= fallDelay)
            {
                isFalling = true;
                respawnTimer = 0f;
            }
        }

        if (isFalling)
        {
            // Падаем вниз
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            respawnTimer += Time.deltaTime;

            // Время восстановления
            if (respawnTimer >= respawnDelay)
            {
                RespawnPlatform();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Игрок наступил на платформу
        if (!isTriggered && collision.gameObject.GetComponent<PlayerHealth>() != null)
        {
            isTriggered = true;
            triggerTimer = 0f;
        }
    }

    /// <summary>
    /// Восстановить платформу
    /// </summary>
    private void RespawnPlatform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isTriggered = false;
        isFalling = false;
        triggerTimer = 0f;
        respawnTimer = 0f;

        if (platformRenderer != null)
        {
            platformRenderer.material.color = originalColor;
        }
    }
}
