using UnityEngine;

/// <summary>
/// Гравитационная зона.
/// При входе игрока принудительно меняет направление гравитации.
/// Фиолетовая полупрозрачная область с частицами.
/// </summary>
public class GravityZone : MonoBehaviour
{
    [Header("Настройки зоны")]
    [Tooltip("Индекс направления гравитации: 0=вниз, 1=влево, 2=вверх, 3=вправо")]
    [SerializeField] private int targetGravityIndex = 2;

    [Tooltip("Восстановить гравитацию при выходе из зоны")]
    [SerializeField] private bool restoreOnExit = false;

    private int previousGravityIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        GravityController gravCtrl = other.GetComponent<GravityController>();
        if (gravCtrl != null)
        {
            // Запоминаем текущее направление для восстановления
            Vector3 currentDir = gravCtrl.GetGravityDirection();
            if (currentDir == Vector3.down) previousGravityIndex = 0;
            else if (currentDir == Vector3.left) previousGravityIndex = 1;
            else if (currentDir == Vector3.up) previousGravityIndex = 2;
            else if (currentDir == Vector3.right) previousGravityIndex = 3;

            // Принудительно меняем гравитацию
            gravCtrl.ForceGravityDirection(targetGravityIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!restoreOnExit) return;

        GravityController gravCtrl = other.GetComponent<GravityController>();
        if (gravCtrl != null)
        {
            gravCtrl.ForceGravityDirection(previousGravityIndex);
        }
    }
}
