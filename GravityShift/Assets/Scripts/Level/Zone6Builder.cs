using UnityEngine;

/// <summary>
/// Зона 6: "Финальный побег"
/// Всё вместе + ограниченные переключения (топливо).
/// Лаборатория разрушается. Хаос.
/// </summary>
public class Zone6Builder : MonoBehaviour
{
    private void Start()
    {
        BuildZone();
        StartCoroutine(Narration());
    }

    private void BuildZone()
    {
        CheckpointSystem cs = GetComponent<CheckpointSystem>();

        // Ограничиваем заряды гравитации
        GravityController gc = FindObjectOfType<GravityController>();
        if (gc != null)
        {
            gc.SetMaxCharges(10);
            gc.SetCooldown(1.5f);
        }

        // Начальная платформа (наклонена — лаборатория ломается)
        GameObject startPlat = LevelBuilder.CreatePlatform(Vector3.zero, new Vector3(6, 1, 6), MaterialFactory.Floor());
        startPlat.transform.rotation = Quaternion.Euler(0, 0, 3); // Лёгкий наклон

        // === Секция 1: Бег по разрушающимся платформам ===
        for (int i = 0; i < 8; i++)
        {
            float xOffset = Mathf.Sin(i * 0.8f) * 3;
            Vector3 pos = new Vector3(xOffset, Random.Range(-0.5f, 0.5f), 6 + i * 4);
            GameObject fp = LevelBuilder.CreatePlatform(pos, new Vector3(3, 0.5f, 3), MaterialFactory.Falling());
            fp.AddComponent<FallingPlatform>();
            fp.transform.rotation = Quaternion.Euler(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        }

        // Топливо гравитации разбросано
        CreateGravityFuel(new Vector3(0, 1.5f, 14));
        CreateGravityFuel(new Vector3(2, 1.5f, 26));

        // Платформа-чекпоинт
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 40), new Vector3(5, 1, 4), MaterialFactory.Floor());
        LevelBuilder.CreateCheckpoint(new Vector3(0, 1.5f, 40), cs, 0);

        // === Секция 2: Гравитационные зоны + дробилки ===
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 50), new Vector3(8, 1, 16), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(0, 10, 50), new Vector3(8, 1, 16), MaterialFactory.Wall());

        // Авто-переключение зоны (переворачивает на потолок)
        GameObject gz = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gz.transform.position = new Vector3(0, 3, 48);
        gz.transform.localScale = new Vector3(4, 4, 2);
        gz.GetComponent<Renderer>().material = MaterialFactory.GravityZoneMat();
        gz.GetComponent<BoxCollider>().isTrigger = true;
        GravityZone gzone = gz.AddComponent<GravityZone>();

        // Дробилки
        for (int i = 0; i < 4; i++)
        {
            GameObject cr = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cr.transform.position = new Vector3(-3 + i * 2, 9, 50 + i * 3);
            cr.transform.localScale = new Vector3(1.5f, 2, 1.5f);
            cr.GetComponent<Renderer>().material = MaterialFactory.Danger();
            cr.AddComponent<Crusher>();
        }

        // Движущиеся блоки
        for (int i = 0; i < 3; i++)
        {
            GameObject mb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mb.transform.position = new Vector3(-3, 2 + i * 3, 52 + i * 3);
            mb.transform.localScale = new Vector3(2, 2, 2);
            mb.GetComponent<Renderer>().material = MaterialFactory.Danger();
            mb.AddComponent<MovingObstacle>();
        }

        CreateGravityFuel(new Vector3(3, 1.5f, 54));
        LevelBuilder.CreateCoin(new Vector3(0, 5, 50));
        LevelBuilder.CreateCoin(new Vector3(0, 5, 56));

        LevelBuilder.CreatePlatform(new Vector3(0, 0, 64), new Vector3(5, 1, 4), MaterialFactory.Floor());
        LevelBuilder.CreateCheckpoint(new Vector3(0, 1.5f, 64), cs, 0);

        // === Секция 3: Финальный рывок ===
        // Узкий мост с лазерами и толкающими стенами
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 76), new Vector3(3, 1, 20), MaterialFactory.Floor());

        // Толкающие стены
        for (int i = 0; i < 3; i++)
        {
            GameObject pw = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pw.transform.position = new Vector3(-3, 2, 70 + i * 5);
            pw.transform.localScale = new Vector3(2, 4, 2);
            pw.GetComponent<Renderer>().material = MaterialFactory.Pusher();
            pw.AddComponent<PushingWall>();
        }

        // Лазеры
        for (int i = 0; i < 2; i++)
        {
            GameObject laser = new GameObject("Laser_" + i);
            laser.transform.position = new Vector3(-1.5f, 1.5f, 72 + i * 6);
            laser.AddComponent<LaserBeam>();
        }

        CreateGravityFuel(new Vector3(0, 1.5f, 78));

        // === ФИНИШ ===
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 90), new Vector3(6, 1, 6), MaterialFactory.Safe());
        LevelBuilder.CreateFinish(new Vector3(0, 1f, 90));

        // Декоративные "осколки" (лаборатория разрушается)
        for (int i = 0; i < 15; i++)
        {
            GameObject debris = GameObject.CreatePrimitive(PrimitiveType.Cube);
            debris.transform.position = new Vector3(
                Random.Range(-12f, 12f),
                Random.Range(-5f, 15f),
                Random.Range(0f, 90f)
            );
            debris.transform.localScale = Vector3.one * Random.Range(0.3f, 1.2f);
            debris.transform.rotation = Random.rotation;
            debris.GetComponent<Renderer>().material = MaterialFactory.Wall();
            Destroy(debris.GetComponent<Collider>()); // Декоративный
        }
    }

    private void CreateGravityFuel(Vector3 pos)
    {
        GameObject fuel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fuel.transform.position = pos;
        fuel.transform.localScale = Vector3.one * 0.5f;
        fuel.GetComponent<Renderer>().material = MaterialFactory.Collectible();
        fuel.GetComponent<SphereCollider>().isTrigger = true;
        fuel.AddComponent<GravityFuel>();
    }

    private System.Collections.IEnumerator Narration()
    {
        yield return new WaitForSeconds(1f);
        if (NarratorSystem.Instance != null)
        {
            NarratorSystem.Instance.SetGlitchLevel(0.5f);
            NarratorSystem.Instance.ShowMessage("// ВНИМАНИЕ: Критический сбой системы гравитации!");
        }
        yield return new WaitForSeconds(5f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Заряды ограничены. Собирайте топливо. БЕГИТЕ.");
        yield return new WaitForSeconds(20f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Ла...бор...атор...ия... раз...руша...ется...");
    }
}
