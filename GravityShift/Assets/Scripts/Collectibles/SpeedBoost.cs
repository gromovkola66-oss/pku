using UnityEngine;

/// <summary>
/// Бонус скорости.
/// При подборе ускоряет игрока на 5 секунд.
/// Голубое свечение.
/// </summary>
public class SpeedBoost : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Длительность бонуса (сек)")]
    [SerializeField] private float duration = 5f;

    [Tooltip("Скорость вращения")]
    [SerializeField] private float rotateSpeed = 120f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.ActivateSpeedBoost(duration);
            Destroy(gameObject);
        }
    }
}
