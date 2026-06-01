using UnityEngine;

/// <summary>
/// Вращающееся препятствие.
/// Постоянно вращается вокруг заданной оси.
/// При столкновении наносит урон.
/// Rigidbody добавляется автоматически (kinematic) для корректной работы коллизий.
/// </summary>
public class RotatingObstacle : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Ось вращения")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Tooltip("Скорость вращения (градусов/сек)")]
    [SerializeField] private float rotationSpeed = 90f;

    private void Start()
    {
        // Добавляем kinematic Rigidbody для корректной обработки коллизий
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(1);

            // Отталкиваем игрока
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDir = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(pushDir * 10f, ForceMode.Impulse);
            }
        }
    }
}
