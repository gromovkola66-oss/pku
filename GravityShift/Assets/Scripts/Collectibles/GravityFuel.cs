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

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        // Парение
        float y = Mathf.Sin(Time.time * 2.5f) * 0.2f;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + y * Time.deltaTime, transform.localPosition.z);
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
