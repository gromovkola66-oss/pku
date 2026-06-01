using UnityEngine;

/// <summary>
/// Контроллер пост-обработки.
/// Управляет визуальными эффектами: bloom, виньетка, хроматическая аберрация.
/// Использует простые overlay-техники без зависимости от Post Processing Stack.
/// </summary>
public class PostProcessingController : MonoBehaviour
{
    public static PostProcessingController Instance { get; private set; }

    [Header("Настройки эффектов урона")]
    [Tooltip("Длительность эффекта урона")]
    [SerializeField] private float damageEffectDuration = 0.5f;

    [Tooltip("Интенсивность красной виньетки")]
    [SerializeField] private float vignetteIntensity = 0.5f;

    // Состояние
    private float damageTimer = 0f;
    private bool showingDamage = false;

    // UI элементы для эффектов
    private UnityEngine.UI.Image vignetteImage;
    private UnityEngine.UI.Image flashImage;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Привязать UI элементы для эффектов
    /// </summary>
    public void SetEffectImages(UnityEngine.UI.Image vignette, UnityEngine.UI.Image flash)
    {
        vignetteImage = vignette;
        flashImage = flash;
    }

    private void Update()
    {
        if (showingDamage)
        {
            damageTimer -= Time.unscaledDeltaTime;

            // Затухание красной виньетки
            float alpha = (damageTimer / damageEffectDuration) * vignetteIntensity;
            if (vignetteImage != null)
            {
                vignetteImage.color = new Color(1f, 0f, 0f, alpha);
            }

            if (damageTimer <= 0f)
            {
                showingDamage = false;
                if (vignetteImage != null)
                    vignetteImage.color = new Color(1f, 0f, 0f, 0f);
            }
        }
    }

    /// <summary>
    /// Эффект получения урона (красная виньетка)
    /// </summary>
    public void DamageEffect()
    {
        showingDamage = true;
        damageTimer = damageEffectDuration;
    }

    /// <summary>
    /// Эффект лечения (зелёная вспышка)
    /// </summary>
    public void HealEffect()
    {
        if (flashImage != null)
        {
            flashImage.color = new Color(0f, 1f, 0.3f, 0.3f);
            // Затухание через LeanTween или вручную
            StartCoroutine(FadeFlash());
        }
    }

    /// <summary>
    /// Эффект переключения гравитации (синяя вспышка)
    /// </summary>
    public void GravitySwitchFlash()
    {
        if (flashImage != null)
        {
            flashImage.color = new Color(0.2f, 0.5f, 1f, 0.2f);
            StartCoroutine(FadeFlash());
        }
    }

    private System.Collections.IEnumerator FadeFlash()
    {
        if (flashImage == null) yield break;

        Color c = flashImage.color;
        while (c.a > 0.01f)
        {
            c.a -= Time.unscaledDeltaTime * 2f;
            flashImage.color = c;
            yield return null;
        }
        flashImage.color = new Color(0, 0, 0, 0);
    }
}
