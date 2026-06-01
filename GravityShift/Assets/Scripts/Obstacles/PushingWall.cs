using UnityEngine;

/// <summary>
/// Толкающая стена.
/// Периодически выдвигается, пытаясь столкнуть игрока.
/// </summary>
public class PushingWall : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Направление толчка")]
    [SerializeField] private Vector3 pushDirection = Vector3.forward;

    [Tooltip("Расстояние выдвижения")]
    [SerializeField] private float pushDistance = 4f;

    [Tooltip("Скорость выдвижения")]
    [SerializeField] private float pushSpeed = 8f;

    [Tooltip("Скорость возврата")]
    [SerializeField] private float retractSpeed = 2f;

    [Tooltip("Интервал между толчками (сек)")]
    [SerializeField] private float interval = 3f;

    [Tooltip("Сила толчка игрока")]
    [SerializeField] private float playerPushForce = 15f;

    // Состояние
    private Vector3 startPosition;
    private Vector3 extendedPosition;
    private float timer = 0f;
    private bool isPushing = false;
    private bool isRetracting = false;

    private void Start()
    {
        startPosition = transform.position;
        extendedPosition = startPosition + pushDirection.normalized * pushDistance;

        // Kinematic Rigidbody для корректных коллизий с игроком
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (!isPushing && !isRetracting)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                isPushing = true;
                timer = 0f;
            }
        }

        if (isPushing)
        {
            transform.position = Vector3.MoveTowards(transform.position, extendedPosition, pushSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, extendedPosition) < 0.01f)
            {
                isPushing = false;
                isRetracting = true;
            }
        }

        if (isRetracting)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, retractSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.01f)
            {
                isRetracting = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRb != null && collision.gameObject.GetComponent<PlayerHealth>() != null)
        {
            // Толкаем игрока
            playerRb.AddForce(pushDirection.normalized * playerPushForce, ForceMode.Impulse);
        }
    }
}
