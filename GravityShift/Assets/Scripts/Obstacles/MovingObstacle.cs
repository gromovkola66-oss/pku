using UnityEngine;

/// <summary>
/// Движущееся препятствие.
/// Перемещается туда-сюда между двумя точками.
/// При столкновении наносит урон игроку.
/// </summary>
public class MovingObstacle : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("Направление движения")]
    [SerializeField] private Vector3 moveDirection = Vector3.right;

    [Tooltip("Расстояние движения")]
    [SerializeField] private float distance = 5f;

    [Tooltip("Скорость движения")]
    [SerializeField] private float speed = 3f;

    [Tooltip("Пауза на конечных точках (сек)")]
    [SerializeField] private float pauseTime = 0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float pauseTimer = 0f;
    private bool movingToEnd = true;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + moveDirection.normalized * distance;
    }

    private void Update()
    {
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        // Движение между точками
        Vector3 target = movingToEnd ? endPosition : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Достигли конечной точки
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            movingToEnd = !movingToEnd;
            pauseTimer = pauseTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Наносим урон игроку при столкновении
        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(1);
        }
    }
}
