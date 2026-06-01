using UnityEngine;
using System;

/// <summary>
/// Инвентарь игрока: монеты, бонусы, счёт.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("Счёт")]
    [SerializeField] private int coins = 0;

    [Header("Бонусы")]
    [SerializeField] private float speedBoostTimer = 0f;
    [SerializeField] private float timeSlowTimer = 0f;

    [Tooltip("Множитель скорости при бонусе")]
    [SerializeField] private float speedBoostMultiplier = 1.5f;

    [Tooltip("Масштаб времени при замедлении")]
    [SerializeField] private float timeSlowScale = 0.5f;

    // События
    public event Action<int> OnCoinsChanged;
    public event Action OnSpeedBoostStart;
    public event Action OnSpeedBoostEnd;
    public event Action OnTimeSlowStart;
    public event Action OnTimeSlowEnd;

    /// <summary>
    /// Текущее количество монет
    /// </summary>
    public int Coins => coins;

    /// <summary>
    /// Активен ли бонус скорости
    /// </summary>
    public bool IsSpeedBoosted => speedBoostTimer > 0f;

    /// <summary>
    /// Активно ли замедление времени
    /// </summary>
    public bool IsTimeSlowed => timeSlowTimer > 0f;

    /// <summary>
    /// Множитель скорости (1.0 если нет бонуса)
    /// </summary>
    public float SpeedMultiplier => IsSpeedBoosted ? speedBoostMultiplier : 1f;

    private void Update()
    {
        // Обновление таймера бонуса скорости
        if (speedBoostTimer > 0f)
        {
            speedBoostTimer -= Time.unscaledDeltaTime;
            if (speedBoostTimer <= 0f)
            {
                speedBoostTimer = 0f;
                OnSpeedBoostEnd?.Invoke();
            }
        }

        // Обновление таймера замедления времени
        if (timeSlowTimer > 0f)
        {
            timeSlowTimer -= Time.unscaledDeltaTime;
            if (timeSlowTimer <= 0f)
            {
                timeSlowTimer = 0f;
                Time.timeScale = 1f;
                OnTimeSlowEnd?.Invoke();
            }
        }
    }

    /// <summary>
    /// Добавить монеты
    /// </summary>
    public void AddCoins(int amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke(coins);
    }

    /// <summary>
    /// Активировать бонус скорости
    /// </summary>
    public void ActivateSpeedBoost(float duration = 5f)
    {
        speedBoostTimer = duration;
        OnSpeedBoostStart?.Invoke();
    }

    /// <summary>
    /// Активировать замедление времени
    /// </summary>
    public void ActivateTimeSlow(float duration = 5f)
    {
        timeSlowTimer = duration;
        Time.timeScale = timeSlowScale;
        OnTimeSlowStart?.Invoke();
    }

    /// <summary>
    /// Сбросить инвентарь
    /// </summary>
    public void Reset()
    {
        coins = 0;
        speedBoostTimer = 0f;
        timeSlowTimer = 0f;
        Time.timeScale = 1f;
        OnCoinsChanged?.Invoke(coins);
    }
}
