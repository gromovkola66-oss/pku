using UnityEngine;

/// <summary>
/// Гравитационное топливо.
/// Восстанавливает заряды переключения гравитации.
/// Бело-золотое свечение.
/// </summary>
public class GravityFuel : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Количество восстанавливаемых зарядов")]
    [SerializeField] private int chargesRestored = 3;

    [Tooltip("Скорость вращения")]
    [SerializeField] private float rotateSpeed = 200f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        // Парение вверх-вниз (синусоида от начальной позиции)
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(Time.time * 2.5f) * 0.2f;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        GravityController gravCtrl = other.GetComponent<GravityController>();
        if (gravCtrl != null)
        {
            gravCtrl.AddCharges(chargesRestored);
            Destroy(gameObject);
        }
    }
}
