using UnityEngine;

/// <summary>
/// Монета. Вращается, парит вверх-вниз.
/// При подборе добавляет очки в инвентарь.
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Количество очков за монету")]
    [SerializeField] private int value = 10;

    [Tooltip("Скорость вращения")]
    [SerializeField] private float rotateSpeed = 180f;

    [Tooltip("Амплитуда парения")]
    [SerializeField] private float bobAmplitude = 0.3f;

    [Tooltip("Скорость парения")]
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Вращение
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        // Парение вверх-вниз
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddCoins(value);
            Destroy(gameObject);
        }
    }
}
