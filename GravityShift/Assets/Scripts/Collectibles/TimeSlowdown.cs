using UnityEngine;

/// <summary>
/// Замедление времени.
/// При подборе замедляет все препятствия на 5 секунд.
/// Фиолетовое свечение.
/// </summary>
public class TimeSlowdown : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Длительность замедления (сек)")]
    [SerializeField] private float duration = 5f;

    [Tooltip("Скорость вращения")]
    [SerializeField] private float rotateSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.ActivateTimeSlow(duration);
            Destroy(gameObject);
        }
    }
}
