using UnityEngine;
using System;

/// <summary>
/// Система здоровья игрока.
/// 3 жизни по умолчанию, неуязвимость после урона, щит.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Здоровье")]
    [Tooltip("Максимальное здоровье")]
    [SerializeField] private int maxHealth = 3;

    [Tooltip("Время неуязвимости после урона (сек)")]
    [SerializeField] private float invulnerabilityTime = 1.5f;

    [Header("Состояние")]
    [SerializeField] private int currentHealth;
    [SerializeField] private bool hasShield = false;

    private float lastDamageTime = -10f;
    private bool isDead = false;

    // События
    public event Action<int> OnHealthChanged;
    public event Action OnDamaged;
    public event Action OnDeath;
    public event Action OnShieldBroken;

    /// <summary>
    /// Текущее здоровье
    /// </summary>
    public int CurrentHealth => currentHealth;

    /// <summary>
    /// Максимальное здоровье
    /// </summary>
    public int MaxHealth => maxHealth;

    /// <summary>
    /// Есть ли щит
    /// </summary>
    public bool HasShield => hasShield;

    /// <summary>
    /// Жив ли игрок
    /// </summary>
    public bool IsAlive => !isDead;

    private void Start()
    {
        currentHealth = maxHealth;

        // Подписываемся на DamageEffect
        OnDamaged += () =>
        {
            if (PostProcessingController.Instance != null)
                PostProcessingController.Instance.DamageEffect();
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayDamage();
            ParticleFactory.CreateDamageParticles(transform.position);
        };
    }

    /// <summary>
    /// Получить урон
    /// </summary>
    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;

        // Проверка неуязвимости
        if (Time.time - lastDamageTime < invulnerabilityTime) return;

        // Щит поглощает урон
        if (hasShield)
        {
            hasShield = false;
            lastDamageTime = Time.time;
            OnShieldBroken?.Invoke();
            return;
        }

        currentHealth -= amount;
        lastDamageTime = Time.time;

        OnHealthChanged?.Invoke(currentHealth);
        OnDamaged?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Вылечить игрока
    /// </summary>
    public void Heal(int amount = 1)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Полностью восстановить здоровье (на чекпоинте)
    /// </summary>
    public void FullHeal()
    {
        currentHealth = maxHealth;
        isDead = false;
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Дать щит
    /// </summary>
    public void GiveShield()
    {
        hasShield = true;
    }

    /// <summary>
    /// Смерть игрока
    /// </summary>
    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
    }

    /// <summary>
    /// Воскресить игрока (при респауне)
    /// </summary>
    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        hasShield = false;
        OnHealthChanged?.Invoke(currentHealth);
    }
}
