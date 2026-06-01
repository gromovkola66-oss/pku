using UnityEngine;

/// <summary>
/// Зона 3: "Куб"
/// Кубическая комната, путь проходит по всем 4 стенам.
/// Каждая поверхность — свой участок с препятствиями.
/// </summary>
public class Zone3Builder : MonoBehaviour
{
    private void Start()
    {
        BuildZone();
        StartCoroutine(Narration());
    }

    private void BuildZone()
    {
        CheckpointSystem cs = GetComponent<CheckpointSystem>();
        float roomSize = 20f;
        float halfSize = roomSize / 2f;

        // === Создаём куб-комнату ===

        // Пол
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 0), new Vector3(roomSize, 1, roomSize), MaterialFactory.Floor());

        // Потолок
        LevelBuilder.CreatePlatform(new Vector3(0, roomSize, 0), new Vector3(roomSize, 1, roomSize), MaterialFactory.Floor());

        // Левая стена
        LevelBuilder.CreateWall(new Vector3(-halfSize - 0.5f, halfSize, 0), new Vector3(1, roomSize, roomSize));

        // Правая стена
        LevelBuilder.CreateWall(new Vector3(halfSize + 0.5f, halfSize, 0), new Vector3(1, roomSize, roomSize));

        // Задняя стена
        LevelBuilder.CreateWall(new Vector3(0, halfSize, -halfSize - 0.5f), new Vector3(roomSize, roomSize, 1));

        // Передняя стена (с проёмами)
        LevelBuilder.CreateWall(new Vector3(-6, halfSize, halfSize + 0.5f), new Vector3(8, roomSize, 1));
        LevelBuilder.CreateWall(new Vector3(6, halfSize, halfSize + 0.5f), new Vector3(8, roomSize, 1));

        // Неоновые линии на рёбрах куба
        LevelBuilder.CreateNeonStrip(new Vector3(-halfSize, 0.5f, -halfSize), new Vector3(0.15f, 0.15f, roomSize));
        LevelBuilder.CreateNeonStrip(new Vector3(halfSize, 0.5f, -halfSize), new Vector3(0.15f, 0.15f, roomSize));
        LevelBuilder.CreateNeonStrip(new Vector3(-halfSize, roomSize - 0.5f, -halfSize), new Vector3(0.15f, 0.15f, roomSize));
        LevelBuilder.CreateNeonStrip(new Vector3(halfSize, roomSize - 0.5f, -halfSize), new Vector3(0.15f, 0.15f, roomSize));

        // === ПУТЬ ПО ПОЛУ ===
        // Вращающиеся балки на полу
        GameObject floorBar1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floorBar1.transform.position = new Vector3(0, 1.5f, -5);
        floorBar1.transform.localScale = new Vector3(8, 0.5f, 0.5f);
        floorBar1.GetComponent<Renderer>().material = MaterialFactory.Danger();
        RotatingObstacle ro1 = floorBar1.AddComponent<RotatingObstacle>();

        GameObject floorBar2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floorBar2.transform.position = new Vector3(0, 1.5f, 3);
        floorBar2.transform.localScale = new Vector3(8, 0.5f, 0.5f);
        floorBar2.GetComponent<Renderer>().material = MaterialFactory.Danger();
        floorBar2.AddComponent<RotatingObstacle>();

        // Монеты на полу
        LevelBuilder.CreateCoin(new Vector3(3, 1.5f, -3));
        LevelBuilder.CreateCoin(new Vector3(-3, 1.5f, 1));

        // === ПУТЬ ПО ПРАВОЙ СТЕНЕ ===
        // Платформы на правой стене (гравитация вправо = стена становится полом)
        LevelBuilder.CreatePlatform(new Vector3(halfSize - 1, 4, -5), new Vector3(2, 1, 3), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(halfSize - 1, 8, 0), new Vector3(2, 1, 3), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(halfSize - 1, 12, 5), new Vector3(2, 1, 3), MaterialFactory.Floor());

        // Движущийся блок на стене
        GameObject wallBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallBlock.transform.position = new Vector3(halfSize - 2, 10, 0);
        wallBlock.transform.localScale = new Vector3(1, 1, 3);
        wallBlock.GetComponent<Renderer>().material = MaterialFactory.Danger();
        wallBlock.AddComponent<MovingObstacle>();

        LevelBuilder.CreateCoin(new Vector3(halfSize - 2, 6, -5));
        LevelBuilder.CreateCoin(new Vector3(halfSize - 2, 14, 5));

        // Чекпоинт на стене
        LevelBuilder.CreateCheckpoint(new Vector3(halfSize - 2, 14, 5), cs, 3); // gravity right

        // === ПУТЬ ПО ПОТОЛКУ ===
        // Движущиеся блоки на потолке
        GameObject ceilBlock1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceilBlock1.transform.position = new Vector3(-3, roomSize - 1.5f, 0);
        ceilBlock1.transform.localScale = new Vector3(2, 2, 2);
        ceilBlock1.GetComponent<Renderer>().material = MaterialFactory.Danger();
        MovingObstacle ceilMo = ceilBlock1.AddComponent<MovingObstacle>();

        GameObject ceilBlock2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceilBlock2.transform.position = new Vector3(3, roomSize - 1.5f, -4);
        ceilBlock2.transform.localScale = new Vector3(2, 2, 2);
        ceilBlock2.GetComponent<Renderer>().material = MaterialFactory.Danger();
        ceilBlock2.AddComponent<MovingObstacle>();

        LevelBuilder.CreateCoin(new Vector3(0, roomSize - 2, 3));
        LevelBuilder.CreateCoin(new Vector3(0, roomSize - 2, -3));

        // === ПУТЬ ПО ЛЕВОЙ СТЕНЕ (финальный) ===
        LevelBuilder.CreatePlatform(new Vector3(-halfSize + 1, 16, 0), new Vector3(2, 1, 3), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(-halfSize + 1, 12, -4), new Vector3(2, 1, 3), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(-halfSize + 1, 8, 2), new Vector3(2, 1, 3), MaterialFactory.Floor());

        // Чекпоинт
        LevelBuilder.CreateCheckpoint(new Vector3(-halfSize + 2, 8, 2), cs, 1); // gravity left

        // === ФИНИШ (на полу, в центре) ===
        LevelBuilder.CreateFinish(new Vector3(0, 0.6f, 8));

        // Бонус скорости
        GameObject speedBonus = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        speedBonus.transform.position = new Vector3(0, roomSize - 2, 0);
        speedBonus.transform.localScale = Vector3.one * 0.6f;
        speedBonus.GetComponent<Renderer>().material = MaterialFactory.Collectible();
        speedBonus.GetComponent<SphereCollider>().isTrigger = true;
        speedBonus.AddComponent<SpeedBoost>();
    }

    private System.Collections.IEnumerator Narration()
    {
        yield return new WaitForSeconds(2f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Зона 3: Куб. Путь проходит по всем поверхностям.");

        yield return new WaitForSeconds(7f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Переключайте гравитацию на стены. Каждая поверхность — новый путь.");
    }
}
