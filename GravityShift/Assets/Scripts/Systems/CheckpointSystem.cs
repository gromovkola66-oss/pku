using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Система чекпоинтов.
/// Сохраняет позицию, поворот и направление гравитации.
/// Восстанавливает здоровье при активации.
/// </summary>
public class CheckpointSystem : MonoBehaviour
{
    /// <summary>
    /// Данные для респауна
    /// </summary>
    [System.Serializable]
    public struct RespawnData
    {
        public Vector3 position;
        public Quaternion rotation;
        public int gravityIndex;
    }

    [Header("Настройки")]
    [Tooltip("Список чекпоинтов")]
    [SerializeField] private List<RespawnData> checkpoints = new List<RespawnData>();

    private int lastCheckpointIndex = 0;

    private void Start()
    {
        // Начальная точка всегда первый чекпоинт
        if (checkpoints.Count == 0)
        {
            checkpoints.Add(new RespawnData
            {
                position = Vector3.up * 2f,
                rotation = Quaternion.identity,
                gravityIndex = 0
            });
        }
    }

    /// <summary>
    /// Добавить чекпоинт
    /// </summary>
    public void AddCheckpoint(Vector3 position, Quaternion rotation, int gravityIndex)
    {
        checkpoints.Add(new RespawnData
        {
            position = position,
            rotation = rotation,
            gravityIndex = gravityIndex
        });
    }

    /// <summary>
    /// Установить последний активированный чекпоинт
    /// </summary>
    public void SetCheckpoint(int index)
    {
        if (index >= 0 && index < checkpoints.Count)
        {
            lastCheckpointIndex = index;
        }
    }

    /// <summary>
    /// Активировать чекпоинт (вызывается триггером)
    /// </summary>
    public void ActivateCheckpoint(Vector3 position, Quaternion rotation, int gravityIndex)
    {
        // Проверяем, не активирован ли уже этот чекпоинт
        for (int i = 0; i < checkpoints.Count; i++)
        {
            if (Vector3.Distance(checkpoints[i].position, position) < 1f)
            {
                lastCheckpointIndex = i;
                return;
            }
        }

        // Новый чекпоинт
        AddCheckpoint(position, rotation, gravityIndex);
        lastCheckpointIndex = checkpoints.Count - 1;

        // Восстанавливаем здоровье игрока
        PlayerHealth health = FindObjectOfType<PlayerHealth>();
        if (health != null) health.FullHeal();
    }

    /// <summary>
    /// Получить данные для респауна
    /// </summary>
    public RespawnData GetRespawnData()
    {
        if (lastCheckpointIndex < checkpoints.Count)
        {
            return checkpoints[lastCheckpointIndex];
        }
        return new RespawnData
        {
            position = Vector3.up * 2f,
            rotation = Quaternion.identity,
            gravityIndex = 0
        };
    }
}
