using UnityEngine;
using System;

/// <summary>
/// Система переключения гравитации.
/// Q - поворот гравитации влево, E - вправо.
/// 4 направления: вниз, влево, вверх, вправо.
/// Плавная ротация камеры при переключении.
/// </summary>
public class GravityController : MonoBehaviour
{
    [Header("Настройки гравитации")]
    [Tooltip("Сила гравитации")]
    [SerializeField] private float gravityStrength = 19.62f;

    [Tooltip("Время кулдауна между переключениями (сек)")]
    [SerializeField] private float switchCooldown = 0.5f;

    [Tooltip("Скорость поворота при переключении")]
    [SerializeField] private float rotationSpeed = 8f;

    [Tooltip("Максимальное количество переключений (-1 = бесконечно)")]
    [SerializeField] private int maxCharges = -1;

    [Header("Состояние")]
    [SerializeField] private int currentCharges = -1;

    // Направления гравитации (по часовой: вниз, лево, верх, право)
    private Vector3[] gravityDirections = new Vector3[]
    {
        Vector3.down,   // 0 - нормальная гравитация
        Vector3.left,   // 1 - гравитация влево
        Vector3.up,     // 2 - гравитация вверх (на потолке)
        Vector3.right   // 3 - гравитация вправо
    };

    private int currentDirectionIndex = 0;
    private float lastSwitchTime = -10f;
    private Quaternion targetRotation;
    private bool isRotating = false;

    // События
    public event Action<Vector3> OnGravityChanged;
    public event Action<int> OnChargesChanged;

    /// <summary>
    /// Текущее направление "вверх" для игрока
    /// </summary>
    public Vector3 GetCurrentUp()
    {
        return -gravityDirections[currentDirectionIndex];
    }

    /// <summary>
    /// Получить текущее направление гравитации
    /// </summary>
    public Vector3 GetGravityDirection()
    {
        return gravityDirections[currentDirectionIndex];
    }

    /// <summary>
    /// Получить оставшийся кулдаун (0-1)
    /// </summary>
    public float GetCooldownProgress()
    {
        float elapsed = Time.time - lastSwitchTime;
        return Mathf.Clamp01(elapsed / switchCooldown);
    }

    /// <summary>
    /// Получить текущие заряды
    /// </summary>
    public int GetCurrentCharges()
    {
        return currentCharges;
    }

    /// <summary>
    /// Добавить заряды (для подбора GravityFuel)
    /// </summary>
    public void AddCharges(int amount)
    {
        if (maxCharges > 0)
        {
            currentCharges = Mathf.Min(currentCharges + amount, maxCharges);
            OnChargesChanged?.Invoke(currentCharges);
        }
    }

    /// <summary>
    /// Установить лимит зарядов
    /// </summary>
    public void SetMaxCharges(int max)
    {
        maxCharges = max;
        currentCharges = max;
        OnChargesChanged?.Invoke(currentCharges);
    }

    /// <summary>
    /// Установить кулдаун
    /// </summary>
    public void SetCooldown(float cooldown)
    {
        switchCooldown = cooldown;
    }

    private void Start()
    {
        targetRotation = transform.rotation;
        currentCharges = maxCharges;
        ApplyGravity();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        // Обработка ввода переключения гравитации
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchGravity(-1); // Влево (против часовой)
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchGravity(1); // Вправо (по часовой)
        }

        // Плавный поворот к целевой ротации
        if (isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Проверяем, достигли ли целевого поворота
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }

    /// <summary>
    /// Переключить гравитацию
    /// direction: -1 = влево (Q), +1 = вправо (E)
    /// </summary>
    private void SwitchGravity(int direction)
    {
        // Проверка кулдауна
        if (Time.time - lastSwitchTime < switchCooldown) return;

        // Проверка зарядов
        if (maxCharges > 0 && currentCharges <= 0) return;

        // Меняем индекс направления
        currentDirectionIndex = (currentDirectionIndex + direction + 4) % 4;
        lastSwitchTime = Time.time;

        // Уменьшаем заряды
        if (maxCharges > 0)
        {
            currentCharges--;
            OnChargesChanged?.Invoke(currentCharges);
        }

        // Применяем гравитацию
        ApplyGravity();

        // Вычисляем целевой поворот (игрок должен быть "ногами к земле")
        Vector3 newUp = GetCurrentUp();
        targetRotation = Quaternion.FromToRotation(transform.up, newUp) * transform.rotation;
        isRotating = true;

        // Вызываем событие
        OnGravityChanged?.Invoke(gravityDirections[currentDirectionIndex]);
    }

    /// <summary>
    /// Применить текущую гравитацию к физике
    /// </summary>
    private void ApplyGravity()
    {
        Physics.gravity = gravityDirections[currentDirectionIndex] * gravityStrength;
    }

    /// <summary>
    /// Принудительно установить направление гравитации (для гравитационных зон)
    /// </summary>
    public void ForceGravityDirection(int directionIndex)
    {
        if (directionIndex < 0 || directionIndex >= 4) return;
        if (directionIndex == currentDirectionIndex) return;

        currentDirectionIndex = directionIndex;
        lastSwitchTime = Time.time;

        ApplyGravity();

        Vector3 newUp = GetCurrentUp();
        targetRotation = Quaternion.FromToRotation(transform.up, newUp) * transform.rotation;
        isRotating = true;

        OnGravityChanged?.Invoke(gravityDirections[currentDirectionIndex]);
    }
}
