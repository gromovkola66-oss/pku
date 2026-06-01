using UnityEngine;

/// <summary>
/// Координатор эффектов при переключении гравитации.
/// Управляет частицами, тряской и визуальными эффектами.
/// </summary>
public class GravitySwitchEffect : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Интенсивность тряски при переключении")]
    [SerializeField] private float shakeIntensity = 0.15f;

    [Tooltip("Длительность тряски")]
    [SerializeField] private float shakeDuration = 0.2f;

    private GravityController gravityController;
    private ParticleSystem gravityDust;

    private void Start()
    {
        gravityController = GetComponent<GravityController>();
        if (gravityController != null)
        {
            gravityController.OnGravityChanged += OnGravityChanged;
        }

        // Создаём постоянные частицы пыли
        gravityDust = ParticleFactory.CreateGravityDust(transform);
    }

    /// <summary>
    /// Вызывается при каждом переключении гравитации
    /// </summary>
    private void OnGravityChanged(Vector3 newDirection)
    {
        // Всплеск частиц
        ParticleFactory.CreateSwitchWave(transform.position);

        // Тряска экрана
        if (ScreenShake.Instance != null)
        {
            ScreenShake.Instance.Shake(shakeIntensity, shakeDuration);
        }

        // Обновляем направление пыли
        UpdateDustDirection(newDirection);

        // Звук
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGravitySwitch();
        }
    }

    /// <summary>
    /// Обновить направление частиц пыли
    /// </summary>
    private void UpdateDustDirection(Vector3 gravityDir)
    {
        if (gravityDust == null) return;

        var velocity = gravityDust.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = gravityDir.x * 2f;
        velocity.y = gravityDir.y * 2f;
        velocity.z = gravityDir.z * 2f;
    }

    private void OnDestroy()
    {
        if (gravityController != null)
        {
            gravityController.OnGravityChanged -= OnGravityChanged;
        }
    }
}
