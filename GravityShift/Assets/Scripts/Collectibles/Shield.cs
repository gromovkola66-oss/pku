using UnityEngine;

/// <summary>
/// Подбираемый щит.
/// Даёт защиту от одного удара.
/// Синее свечение, вращение.
/// </summary>
public class Shield : MonoBehaviour
{
    [Tooltip("Скорость вращения")]
    [SerializeField] private float rotateSpeed = 150f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        // Пульсация масштаба
        float scale = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
        transform.localScale = Vector3.one * 0.5f * scale;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.GiveShield();
            Destroy(gameObject);
        }
    }
}
