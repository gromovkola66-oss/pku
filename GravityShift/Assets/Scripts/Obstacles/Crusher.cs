using UnityEngine;

/// <summary>
/// Молот/дробилка.
/// Периодически обрушивается сверху, наносит урон.
/// </summary>
public class Crusher : MonoBehaviour
{
    [Header("Настройки дробилки")]
    [Tooltip("Расстояние удара")]
    [SerializeField] private float crushDistance = 4f;

    [Tooltip("Скорость удара (вниз)")]
    [SerializeField] private float crushSpeed = 20f;

    [Tooltip("Скорость возврата (вверх)")]
    [SerializeField] private float returnSpeed = 3f;

    [Tooltip("Интервал между ударами (сек)")]
    [SerializeField] private float interval = 3f;

    [Tooltip("Пауза внизу (сек)")]
    [SerializeField] private float pauseAtBottom = 0.5f;

    // Состояние
    private Vector3 startPosition;
    private Vector3 bottomPosition;
    private float timer = 0f;
    private float pauseTimer = 0f;
    private enum CrusherState { Waiting, Crushing, Pausing, Returning }
    private CrusherState state = CrusherState.Waiting;

    private void Start()
    {
        startPosition = transform.position;
        bottomPosition = startPosition + Vector3.down * crushDistance;
    }

    private void Update()
    {
        switch (state)
        {
            case CrusherState.Waiting:
                timer += Time.deltaTime;
                if (timer >= interval)
                {
                    state = CrusherState.Crushing;
                    timer = 0f;
                }
                break;

            case CrusherState.Crushing:
                transform.position = Vector3.MoveTowards(transform.position, bottomPosition, crushSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, bottomPosition) < 0.01f)
                {
                    state = CrusherState.Pausing;
                    pauseTimer = 0f;
                }
                break;

            case CrusherState.Pausing:
                pauseTimer += Time.deltaTime;
                if (pauseTimer >= pauseAtBottom)
                {
                    state = CrusherState.Returning;
                }
                break;

            case CrusherState.Returning:
                transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, startPosition) < 0.01f)
                {
                    state = CrusherState.Waiting;
                    timer = 0f;
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == CrusherState.Crushing)
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(2); // Дробилка наносит больше урона
            }
        }
    }
}
