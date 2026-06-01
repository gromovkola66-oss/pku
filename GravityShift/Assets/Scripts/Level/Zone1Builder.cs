using UnityEngine;

/// <summary>
/// Зона 1: "Учебный коридор"
/// Простой коридор с ямами. Игрок учится переключать гравитацию.
/// Перевернись на потолок, чтобы пройти над ямой.
/// </summary>
public class Zone1Builder : MonoBehaviour
{
    private void Start()
    {
        BuildZone();
        ShowIntroNarration();
    }

    private void BuildZone()
    {
        CheckpointSystem cs = GetComponent<CheckpointSystem>();
        float corridorWidth = 6f;
        float wallHeight = 8f;

        // === СЕКЦИЯ 1: Прямой коридор (обучение) ===

        // Пол (первая секция)
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 10), new Vector3(corridorWidth, 1, 20), MaterialFactory.Floor());

        // Стены
        LevelBuilder.CreateWall(new Vector3(-corridorWidth/2 - 0.5f, wallHeight/2, 10), new Vector3(1, wallHeight, 20));
        LevelBuilder.CreateWall(new Vector3(corridorWidth/2 + 0.5f, wallHeight/2, 10), new Vector3(1, wallHeight, 20));

        // Потолок
        LevelBuilder.CreatePlatform(new Vector3(0, wallHeight, 10), new Vector3(corridorWidth, 1, 20), MaterialFactory.Wall());

        // Неоновые акценты на стенах
        LevelBuilder.CreateNeonStrip(new Vector3(-corridorWidth/2 - 0.4f, 2, 10), new Vector3(0.1f, 0.2f, 20));
        LevelBuilder.CreateNeonStrip(new Vector3(corridorWidth/2 + 0.4f, 2, 10), new Vector3(0.1f, 0.2f, 20));

        // === СЕКЦИЯ 2: Первая яма (простая) ===

        // Яма шириной 4 метра
        // Пол до ямы уже создан, создаём пол после ямы
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 26), new Vector3(corridorWidth, 1, 6), MaterialFactory.Floor());

        // Потолок над ямой (чтобы пройти по нему)
        LevelBuilder.CreatePlatform(new Vector3(0, wallHeight, 23), new Vector3(corridorWidth, 1, 6), MaterialFactory.Floor());

        // Стены вдоль ямы
        LevelBuilder.CreateWall(new Vector3(-corridorWidth/2 - 0.5f, wallHeight/2, 23), new Vector3(1, wallHeight, 6));
        LevelBuilder.CreateWall(new Vector3(corridorWidth/2 + 0.5f, wallHeight/2, 23), new Vector3(1, wallHeight, 6));

        // Подсказка (неоновый знак на потолке)
        LevelBuilder.CreateNeonStrip(new Vector3(0, wallHeight - 0.4f, 20), new Vector3(2, 0.1f, 0.5f));

        // === СЕКЦИЯ 3: Вторая яма (длиннее) + монеты ===

        LevelBuilder.CreatePlatform(new Vector3(0, 0, 36), new Vector3(corridorWidth, 1, 8), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(0, wallHeight, 33), new Vector3(corridorWidth, 1, 10), MaterialFactory.Floor());

        // Стены
        LevelBuilder.CreateWall(new Vector3(-corridorWidth/2 - 0.5f, wallHeight/2, 33), new Vector3(1, wallHeight, 10));
        LevelBuilder.CreateWall(new Vector3(corridorWidth/2 + 0.5f, wallHeight/2, 33), new Vector3(1, wallHeight, 10));

        // Монеты на потолке (собираем когда перевернуты)
        LevelBuilder.CreateCoin(new Vector3(-1, wallHeight - 1.5f, 31));
        LevelBuilder.CreateCoin(new Vector3(1, wallHeight - 1.5f, 33));
        LevelBuilder.CreateCoin(new Vector3(0, wallHeight - 1.5f, 35));

        // Чекпоинт
        LevelBuilder.CreateCheckpoint(new Vector3(0, 1.5f, 38), cs, 0);

        // === СЕКЦИЯ 4: Движущийся блок ===

        LevelBuilder.CreatePlatform(new Vector3(0, 0, 50), new Vector3(corridorWidth, 1, 20), MaterialFactory.Floor());
        LevelBuilder.CreatePlatform(new Vector3(0, wallHeight, 50), new Vector3(corridorWidth, 1, 20), MaterialFactory.Wall());
        LevelBuilder.CreateWall(new Vector3(-corridorWidth/2 - 0.5f, wallHeight/2, 50), new Vector3(1, wallHeight, 20));
        LevelBuilder.CreateWall(new Vector3(corridorWidth/2 + 0.5f, wallHeight/2, 50), new Vector3(1, wallHeight, 20));

        // Движущийся блок (горизонтально)
        GameObject movingBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        movingBlock.transform.position = new Vector3(-2, 1.5f, 48);
        movingBlock.transform.localScale = new Vector3(1.5f, 3f, 1.5f);
        movingBlock.GetComponent<Renderer>().material = MaterialFactory.Danger();
        MovingObstacle mo = movingBlock.AddComponent<MovingObstacle>();

        // Второй движущийся блок
        GameObject movingBlock2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        movingBlock2.transform.position = new Vector3(2, 1.5f, 52);
        movingBlock2.transform.localScale = new Vector3(1.5f, 3f, 1.5f);
        movingBlock2.GetComponent<Renderer>().material = MaterialFactory.Danger();
        movingBlock2.AddComponent<MovingObstacle>();

        // Монеты
        LevelBuilder.CreateCoin(new Vector3(0, 1.5f, 46));
        LevelBuilder.CreateCoin(new Vector3(0, 1.5f, 50));
        LevelBuilder.CreateCoin(new Vector3(0, 1.5f, 54));

        // === СЕКЦИЯ 5: Комбинация яма + движущийся блок ===

        // Яма
        LevelBuilder.CreatePlatform(new Vector3(0, wallHeight, 64), new Vector3(corridorWidth, 1, 8), MaterialFactory.Floor());
        LevelBuilder.CreateWall(new Vector3(-corridorWidth/2 - 0.5f, wallHeight/2, 64), new Vector3(1, wallHeight, 8));
        LevelBuilder.CreateWall(new Vector3(corridorWidth/2 + 0.5f, wallHeight/2, 64), new Vector3(1, wallHeight, 8));

        // Движущийся блок на потолке (опасность при переключении вверх)
        GameObject ceilingBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceilingBlock.transform.position = new Vector3(0, wallHeight - 1.5f, 64);
        ceilingBlock.transform.localScale = new Vector3(2f, 1f, 2f);
        ceilingBlock.GetComponent<Renderer>().material = MaterialFactory.Danger();
        MovingObstacle cmo = ceilingBlock.AddComponent<MovingObstacle>();

        // Площадка после ямы
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 72), new Vector3(corridorWidth, 1, 6), MaterialFactory.Floor());
        LevelBuilder.CreateWall(new Vector3(-corridorWidth/2 - 0.5f, wallHeight/2, 72), new Vector3(1, wallHeight, 6));
        LevelBuilder.CreateWall(new Vector3(corridorWidth/2 + 0.5f, wallHeight/2, 72), new Vector3(1, wallHeight, 6));
        LevelBuilder.CreatePlatform(new Vector3(0, wallHeight, 72), new Vector3(corridorWidth, 1, 6), MaterialFactory.Wall());

        // Чекпоинт перед финалом
        LevelBuilder.CreateCheckpoint(new Vector3(0, 1.5f, 73), cs, 0);

        // === ФИНИШ ===
        LevelBuilder.CreatePlatform(new Vector3(0, 0, 80), new Vector3(corridorWidth, 1, 4), MaterialFactory.Safe());
        LevelBuilder.CreateFinish(new Vector3(0, 1f, 80));

        // Стартовая зона (синяя)
        LevelBuilder.CreatePlatform(new Vector3(0, -0.1f, 2), new Vector3(4, 0.2f, 4), MaterialFactory.Checkpoint());
    }

    /// <summary>
    /// Вступительные реплики ИИ-нарратора
    /// </summary>
    private void ShowIntroNarration()
    {
        if (NarratorSystem.Instance == null) return;

        StartCoroutine(NarrationSequence());
    }

    private System.Collections.IEnumerator NarrationSequence()
    {
        yield return new WaitForSeconds(1f);
        NarratorSystem.Instance.ShowMessage("// Инициализация субъекта... Готово.");

        yield return new WaitForSeconds(5f);
        NarratorSystem.Instance.ShowMessage("// Добро пожаловать в лабораторию гравитационных исследований.");

        yield return new WaitForSeconds(6f);
        NarratorSystem.Instance.ShowMessage("// Используйте Q и E для переключения гравитации. Попробуйте.");

        yield return new WaitForSeconds(8f);
        NarratorSystem.Instance.ShowMessage("// Перед вами пропасть. Переключите гравитацию вверх [Q/E] чтобы 'упасть' на потолок.");
    }
}
