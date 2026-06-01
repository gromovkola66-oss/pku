using UnityEngine;

/// <summary>
/// Зона 2: "Шахта"
/// Вертикальный подъём через переключение гравитации.
/// Нужно "падать вверх" через колодец с горизонтальными препятствиями.
/// </summary>
public class Zone2Builder : MonoBehaviour
{
    private void Start()
    {
        BuildZone();
        StartCoroutine(Narration());
    }

    private void BuildZone()
    {
        CheckpointSystem cs = GetComponent<CheckpointSystem>();

        float shaftWidth = 8f;
        float shaftDepth = 8f;

        // === Начальная комната (пол) ===
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 0), new Vector3(shaftWidth, 1, shaftDepth), MaterialFactory.Floor());
        LevelBuilder.CreateWall(new Vector3(-shaftWidth/2 - 0.5f, 5, 0), new Vector3(1, 10, shaftDepth));
        LevelBuilder.CreateWall(new Vector3(shaftWidth/2 + 0.5f, 5, 0), new Vector3(1, 10, shaftDepth));
        LevelBuilder.CreateWall(new Vector3(0, 5, -shaftDepth/2 - 0.5f), new Vector3(shaftWidth, 10, 1));
        LevelBuilder.CreateWall(new Vector3(0, 5, shaftDepth/2 + 0.5f), new Vector3(shaftWidth, 10, 1));

        // === Шахта вверх (высота 60 метров) ===
        float totalHeight = 60f;

        // Стены шахты
        LevelBuilder.CreateWall(new Vector3(-shaftWidth/2 - 0.5f, totalHeight/2, 0), new Vector3(1, totalHeight, shaftDepth));
        LevelBuilder.CreateWall(new Vector3(shaftWidth/2 + 0.5f, totalHeight/2, 0), new Vector3(1, totalHeight, shaftDepth));
        LevelBuilder.CreateWall(new Vector3(0, totalHeight/2, -shaftDepth/2 - 0.5f), new Vector3(shaftWidth, totalHeight, 1));
        LevelBuilder.CreateWall(new Vector3(0, totalHeight/2, shaftDepth/2 + 0.5f), new Vector3(shaftWidth, totalHeight, 1));

        // Неоновые полосы на стенах (ориентиры высоты)
        for (float h = 5; h < totalHeight; h += 10)
        {
            LevelBuilder.CreateNeonStrip(new Vector3(-shaftWidth/2 - 0.4f, h, 0), new Vector3(0.1f, 0.2f, shaftDepth));
            LevelBuilder.CreateNeonStrip(new Vector3(shaftWidth/2 + 0.4f, h, 0), new Vector3(0.1f, 0.2f, shaftDepth));
        }

        // === Платформы-препятствия внутри шахты ===

        // Уровень 1 (высота 8-12): простые платформы с проходом
        LevelBuilder.CreatePlatform(new Vector3(-2, 10, 0), new Vector3(3, 0.5f, shaftDepth), MaterialFactory.Wall());
        LevelBuilder.CreatePlatform(new Vector3(2.5f, 10, 0), new Vector3(2, 0.5f, shaftDepth), MaterialFactory.Wall());
        // Проход посередине

        // Уровень 2 (высота 18): движущаяся платформа
        GameObject moving1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        moving1.transform.position = new Vector3(0, 18, 0);
        moving1.transform.localScale = new Vector3(4, 0.5f, shaftDepth);
        moving1.GetComponent<Renderer>().material = MaterialFactory.Danger();
        MovingObstacle mo1 = moving1.AddComponent<MovingObstacle>();

        LevelBuilder.CreateCoin(new Vector3(0, 15, 0));

        // Чекпоинт на полочке
        LevelBuilder.CreatePlatform(new Vector3(-2, 20, 0), new Vector3(3, 0.5f, 3), MaterialFactory.Floor());
        LevelBuilder.CreateCheckpoint(new Vector3(-2, 21.5f, 0), cs, 0);

        // Уровень 3 (высота 25-30): вращающаяся балка
        GameObject rotBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rotBar.transform.position = new Vector3(0, 28, 0);
        rotBar.transform.localScale = new Vector3(6, 0.5f, 1);
        rotBar.GetComponent<Renderer>().material = MaterialFactory.Danger();
        RotatingObstacle ro = rotBar.AddComponent<RotatingObstacle>();

        LevelBuilder.CreateCoin(new Vector3(2, 25, 2));
        LevelBuilder.CreateCoin(new Vector3(-2, 25, -2));

        // Уровень 4 (высота 35): конвейерные полосы
        GameObject conv = GameObject.CreatePrimitive(PrimitiveType.Cube);
        conv.transform.position = new Vector3(0, 35, 0);
        conv.transform.localScale = new Vector3(shaftWidth - 2, 0.5f, shaftDepth);
        conv.GetComponent<Renderer>().material = MaterialFactory.Conveyor();
        conv.AddComponent<ConveyorBelt>();

        // Проход — нужно переключить на стену чтобы обойти конвейер
        LevelBuilder.CreatePlatform(new Vector3(shaftWidth/2 - 0.5f, 37, 0), new Vector3(0.5f, 3, shaftDepth), MaterialFactory.Floor());

        LevelBuilder.CreateCheckpoint(new Vector3(0, 38.5f, 0), cs, 0);

        // Уровень 5 (высота 42-48): молоты
        GameObject crusher1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crusher1.transform.position = new Vector3(-2, 48, 0);
        crusher1.transform.localScale = new Vector3(3, 2, 3);
        crusher1.GetComponent<Renderer>().material = MaterialFactory.Danger();
        crusher1.AddComponent<Crusher>();

        GameObject crusher2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crusher2.transform.position = new Vector3(2, 50, 0);
        crusher2.transform.localScale = new Vector3(3, 2, 3);
        crusher2.GetComponent<Renderer>().material = MaterialFactory.Danger();
        crusher2.AddComponent<Crusher>();

        LevelBuilder.CreateCoin(new Vector3(0, 45, 0));

        // === Верхняя комната (финиш) ===
        LevelBuilder.CreatePlatform(new Vector3(0, totalHeight, 0), new Vector3(shaftWidth, 1, shaftDepth), MaterialFactory.Safe());
        LevelBuilder.CreateFinish(new Vector3(0, totalHeight + 1, 0));
    }

    private System.Collections.IEnumerator Narration()
    {
        yield return new WaitForSeconds(2f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Зона 2: Шахта. Переключите гравитацию вверх и 'падайте' наверх.");

        yield return new WaitForSeconds(8f);
        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Осторожно с препятствиями. Переключайте на стены чтобы обходить их.");
    }
}
