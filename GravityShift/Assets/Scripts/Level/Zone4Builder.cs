using UnityEngine;

/// <summary>
/// Зона 4: "Лабиринт Эшера"
/// Коридоры на стенах и потолке. Головоломки с кнопками.
/// Цветные поверхности ограничивают переключение гравитации.
/// </summary>
public class Zone4Builder : MonoBehaviour
{
    private void Start()
    {
        BuildZone();
        StartCoroutine(Narration());
    }

    private void BuildZone()
    {
        CheckpointSystem cs = GetComponent<CheckpointSystem>();

        // Начальная комната
        CreateRoom(Vector3.zero, 8, 6, 8);

        // Коридор вперёд (пол)
        CreateCorridor(new Vector3(0, 0, 8), 4, 5, 15, Vector3.forward);

        // Кнопка на потолке (нужно перевернуться чтобы нажать)
        GameObject door1 = LevelBuilder.CreateWall(new Vector3(0, 0, 20), new Vector3(4, 5, 1));
        CreatePressurePlate(new Vector3(0, 4.9f, 12), door1);

        // Коридор на правой стене
        LevelBuilder.CreatePlatform(new Vector3(4, 3, 24), new Vector3(1, 6, 10), MaterialFactory.Floor());
        CreateCorridor(new Vector3(4, 3, 29), 4, 5, 10, Vector3.forward);

        LevelBuilder.CreateCheckpoint(new Vector3(0, 1.5f, 24), cs, 0);

        // Лазер в коридоре (нужно переключить чтобы обойти)
        GameObject laser1 = new GameObject("Laser1");
        laser1.transform.position = new Vector3(0, 2, 28);
        LaserBeam lb = laser1.AddComponent<LaserBeam>();

        // Монеты
        LevelBuilder.CreateCoin(new Vector3(0, 4.5f, 12));
        LevelBuilder.CreateCoin(new Vector3(2, 1.5f, 26));
        LevelBuilder.CreateCoin(new Vector3(-2, 1.5f, 30));

        // Вторая комната с головоломкой
        CreateRoom(new Vector3(0, 0, 38), 10, 8, 10);

        // Дверь на потолке — кнопка на полу
        GameObject door2 = LevelBuilder.CreateWall(new Vector3(0, 7.5f, 43), new Vector3(3, 1, 3));
        CreatePressurePlate(new Vector3(3, 0.1f, 38), door2);

        LevelBuilder.CreateCheckpoint(new Vector3(0, 1.5f, 40), cs, 0);

        // Финальный коридор с появляющимися платформами
        for (int i = 0; i < 5; i++)
        {
            GameObject plat = LevelBuilder.CreatePlatform(
                new Vector3(0, 0, 50 + i * 3),
                new Vector3(3, 0.5f, 2),
                MaterialFactory.Falling());
            AppearingPlatform ap = plat.AddComponent<AppearingPlatform>();
        }

        // Финиш
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 68), new Vector3(6, 1, 4), MaterialFactory.Safe());
        LevelBuilder.CreateFinish(new Vector3(0, 1f, 68));
    }

    private void CreateRoom(Vector3 center, float w, float h, float d)
    {
        float hw = w/2, hh = h/2, hd = d/2;
        LevelBuilder.CreatePlatform(center + new Vector3(0, 0, 0), new Vector3(w, 1, d), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(center + new Vector3(0, h, 0), new Vector3(w, 1, d), MaterialFactory.Wall());
        LevelBuilder.CreateWall(center + new Vector3(-hw-0.5f, hh, 0), new Vector3(1, h, d));
        LevelBuilder.CreateWall(center + new Vector3(hw+0.5f, hh, 0), new Vector3(1, h, d));
        LevelBuilder.CreateWall(center + new Vector3(0, hh, -hd-0.5f), new Vector3(w, h, 1));
    }

    private void CreateCorridor(Vector3 start, float w, float h, float length, Vector3 dir)
    {
        Vector3 center = start + dir * length * 0.5f;
        float hw = w/2;
        LevelBuilder.CreatePlatform(center, new Vector3(w, 1, length), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(center + Vector3.up * h, new Vector3(w, 1, length), MaterialFactory.Wall());
        LevelBuilder.CreateWall(center + new Vector3(-hw-0.5f, h/2, 0), new Vector3(1, h, length));
        LevelBuilder.CreateWall(center + new Vector3(hw+0.5f, h/2, 0), new Vector3(1, h, length));
    }

    private void CreatePressurePlate(Vector3 pos, GameObject linkedDoor)
    {
        GameObject plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plate.transform.position = pos;
        plate.transform.localScale = new Vector3(1.5f, 0.2f, 1.5f);
        plate.GetComponent<Renderer>().material = MaterialFactory.Checkpoint();
        BoxCollider col = plate.GetComponent<BoxCollider>();
        col.isTrigger = true;
        PressurePlate pp = plate.AddComponent<PressurePlate>();
    }

    private System.Collections.IEnumerator Narration()
    {
        yield return new WaitForSeconds(2f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Зона 4: Лабиринт Эшера. Логика — ваш главный инструмент.");
        yield return new WaitForSeconds(7f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Кнопки могут быть на потолке. Двери — где угодно.");
    }
}
