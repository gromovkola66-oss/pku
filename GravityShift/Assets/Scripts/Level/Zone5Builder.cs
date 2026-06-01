using UnityEngine;

/// <summary>
/// Зона 5: "Нулевая точка"
/// Зоны пониженной гравитации, быстрые препятствия.
/// Толкающие стены, падающие платформы, повышенная сложность.
/// </summary>
public class Zone5Builder : MonoBehaviour
{
    private void Start()
    {
        BuildZone();
        StartCoroutine(Narration());
    }

    private void BuildZone()
    {
        CheckpointSystem cs = GetComponent<CheckpointSystem>();
        float corridorWidth = 5f;

        // Увеличиваем кулдаун для этой зоны
        GravityController gc = FindObjectOfType<GravityController>();
        if (gc != null) gc.SetCooldown(1.0f);

        // Начальная платформа
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 0), new Vector3(corridorWidth, 1, 6), MaterialFactory.Floor());

        // === Секция 1: Толкающие стены ===
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 12), new Vector3(corridorWidth, 1, 12), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(0, 6, 12), new Vector3(corridorWidth, 1, 12), MaterialFactory.Wall());

        // Толкающие стены с обеих сторон
        GameObject pw1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pw1.transform.position = new Vector3(-corridorWidth/2 - 1, 3, 10);
        pw1.transform.localScale = new Vector3(2, 5, 2);
        pw1.GetComponent<Renderer>().material = MaterialFactory.Pusher();
        PushingWall push1 = pw1.AddComponent<PushingWall>();

        GameObject pw2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pw2.transform.position = new Vector3(corridorWidth/2 + 1, 3, 14);
        pw2.transform.localScale = new Vector3(2, 5, 2);
        pw2.GetComponent<Renderer>().material = MaterialFactory.Pusher();
        pw2.AddComponent<PushingWall>();

        LevelBuilder.CreateCoin(new Vector3(0, 2, 10));
        LevelBuilder.CreateCoin(new Vector3(0, 2, 14));

        LevelBuilder.CreateCheckpoint(new Vector3(0, 1.5f, 18), cs, 0);

        // === Секция 2: Падающие платформы над пропастью ===
        for (int i = 0; i < 6; i++)
        {
            GameObject fp = LevelBuilder.CreatePlatform(
                new Vector3((i % 2 == 0 ? -1 : 1) * 1.5f, 0, 22 + i * 3),
                new Vector3(2.5f, 0.5f, 2),
                MaterialFactory.Falling());
            fp.AddComponent<FallingPlatform>();
        }

        LevelBuilder.CreateCoin(new Vector3(-1.5f, 1.5f, 25));
        LevelBuilder.CreateCoin(new Vector3(1.5f, 1.5f, 31));

        // Платформа после пропасти
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 42), new Vector3(corridorWidth, 1, 6), MaterialFactory.Floor());
        LevelBuilder.CreateCheckpoint(new Vector3(0, 1.5f, 42), cs, 0);

        // === Секция 3: Дробилки + конвейер ===
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 52), new Vector3(corridorWidth, 1, 14), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(0, 8, 52), new Vector3(corridorWidth, 1, 14), MaterialFactory.Wall());

        // Конвейер пытается затащить под дробилку
        GameObject conv = GameObject.CreatePrimitive(PrimitiveType.Cube);
        conv.transform.position = new Vector3(0, 0.6f, 52);
        conv.transform.localScale = new Vector3(corridorWidth - 1, 0.2f, 14);
        conv.GetComponent<Renderer>().material = MaterialFactory.Conveyor();
        conv.AddComponent<ConveyorBelt>();

        // Дробилки
        for (int i = 0; i < 3; i++)
        {
            GameObject cr = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cr.transform.position = new Vector3(0, 7.5f, 48 + i * 4);
            cr.transform.localScale = new Vector3(corridorWidth - 1, 2, 2);
            cr.GetComponent<Renderer>().material = MaterialFactory.Danger();
            cr.AddComponent<Crusher>();
        }

        LevelBuilder.CreateCoin(new Vector3(0, 2, 50));
        LevelBuilder.CreateCoin(new Vector3(0, 2, 54));

        // Щит перед финальной секцией
        GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shield.transform.position = new Vector3(0, 1.5f, 58);
        shield.transform.localScale = Vector3.one * 0.5f;
        shield.GetComponent<Renderer>().material = MaterialFactory.ShieldMat();
        shield.GetComponent<SphereCollider>().isTrigger = true;
        shield.AddComponent<Shield>();

        // === Финиш ===
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 64), new Vector3(corridorWidth, 1, 4), MaterialFactory.Safe());
        LevelBuilder.CreateFinish(new Vector3(0, 1f, 64));
    }

    private System.Collections.IEnumerator Narration()
    {
        yield return new WaitForSeconds(2f);
        if (NarratorSystem.Instance != null)
        {
            NarratorSystem.Instance.SetGlitchLevel(0.2f);
            NarratorSystem.Instance.ShowMessage("// Зона 5: Нулевая точка. Системы нестабильны...");
        }
        yield return new WaitForSeconds(7f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Кулдаун переключения увеличен. Планируйте действия.");
    }
}
