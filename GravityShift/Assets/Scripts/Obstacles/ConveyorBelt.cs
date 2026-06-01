using UnityEngine;

/// <summary>
/// Конвейерная лента.
/// Толкает игрока в заданном направлении, пока он стоит на ней.
/// </summary>
public class ConveyorBelt : MonoBehaviour
{
    [Header("Настройки конвейера")]
    [Tooltip("Направление движения ленты")]
    [SerializeField] private Vector3 beltDirection = Vector3.forward;

    [Tooltip("Скорость ленты")]
    [SerializeField] private float beltSpeed = 5f;

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null && collision.gameObject.GetComponent<PlayerHealth>() != null)
        {
            // Прикладываем силу в направлении ленты
            Vector3 worldDirection = transform.TransformDirection(beltDirection.normalized);
            rb.AddForce(worldDirection * beltSpeed, ForceMode.Acceleration);
        }
    }
}
