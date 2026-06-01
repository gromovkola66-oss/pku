using UnityEngine;

/// <summary>
/// Кнопка давления (на полу/стене/потолке).
/// При нажатии активирует связанный объект (дверь и т.д.).
/// Визуально утапливается при нажатии.
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Объект, который активируется при нажатии")]
    [SerializeField] private GameObject linkedObject;

    [Tooltip("Глубина нажатия")]
    [SerializeField] private float pressDepth = 0.1f;

    [Tooltip("Остаётся ли нажатой после активации")]
    [SerializeField] private bool stayPressed = false;

    // Состояние
    private Vector3 originalPosition;
    private bool isPressed = false;
    private int objectsOnPlate = 0;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() != null || other.GetComponent<Rigidbody>() != null)
        {
            objectsOnPlate++;
            if (!isPressed)
            {
                Press();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (stayPressed) return;

        if (other.GetComponent<PlayerHealth>() != null || other.GetComponent<Rigidbody>() != null)
        {
            objectsOnPlate--;
            if (objectsOnPlate <= 0)
            {
                Release();
            }
        }
    }

    /// <summary>
    /// Нажать кнопку
    /// </summary>
    private void Press()
    {
        isPressed = true;
        transform.position = originalPosition - transform.up * pressDepth;

        // Активируем связанный объект (деактивируем его — "открываем дверь")
        if (linkedObject != null)
        {
            linkedObject.SetActive(false);
        }
    }

    /// <summary>
    /// Отпустить кнопку
    /// </summary>
    private void Release()
    {
        isPressed = false;
        transform.position = originalPosition;
        objectsOnPlate = 0;

        if (linkedObject != null)
        {
            linkedObject.SetActive(true);
        }
    }
}
