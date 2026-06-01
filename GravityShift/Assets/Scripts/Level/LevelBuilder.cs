using UnityEngine;

/// <summary>
/// Главный строитель уровня.
/// Создаёт игрока, UI, освещение и вызывает нужный ZoneBuilder.
/// Запускается при старте сцены.
/// </summary>
public class LevelBuilder : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Номер зоны для загрузки (1-6)")]
    [SerializeField] private int zoneToLoad = 1;

    private void Awake()
    {
        // Очищаем кеш материалов
        MaterialFactory.ClearCache();

        // Создаём основные компоненты
        CreateLighting();
        CreatePlayer();
        CreateSystems();
        BuildZone();
    }

    /// <summary>
    /// Создать освещение сцены
    /// </summary>
    private void CreateLighting()
    {
        // Направленный свет (солнце)
        GameObject lightObj = new GameObject("DirectionalLight");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        light.color = new Color(0.9f, 0.9f, 1f);
        light.shadows = LightShadows.Soft;
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Ambient цвет
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.2f, 0.22f, 0.3f);

        // Skybox (градиентный цвет фона) — настраиваем на камере после её создания
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.05f, 0.05f, 0.15f);
        }
    }

    /// <summary>
    /// Создать игрока
    /// </summary>
    private void CreatePlayer()
    {
        // Капсула как тело игрока
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0, 2f, 0);
        player.transform.localScale = new Vector3(0.8f, 1f, 0.8f);

        // Невидимый (камера от первого лица)
        Renderer playerRenderer = player.GetComponent<Renderer>();
        playerRenderer.enabled = false;

        // Rigidbody
        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 70f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Скрипты
        player.AddComponent<FirstPersonController>();
        player.AddComponent<GravityController>();
        player.AddComponent<PlayerHealth>();
        player.AddComponent<PlayerInventory>();
        player.AddComponent<GravitySwitchEffect>();

        // Камера (дочерний объект, на уровне "глаз")
        GameObject cameraObj = new GameObject("PlayerCamera");
        cameraObj.transform.SetParent(player.transform);
        cameraObj.transform.localPosition = new Vector3(0, 0.6f, 0);

        Camera cam = cameraObj.AddComponent<Camera>();
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 500f;
        cam.fieldOfView = 75f;

        // Удаляем Main Camera если есть
        if (Camera.main != null && Camera.main.gameObject != cameraObj)
        {
            Destroy(Camera.main.gameObject);
        }

        // ScreenShake на камеру
        ScreenShake shake = cameraObj.AddComponent<ScreenShake>();
        shake.SetCamera(cameraObj.transform);

        // AudioListener на камеру
        cameraObj.AddComponent<AudioListener>();
    }

    /// <summary>
    /// Создать системные объекты (менеджеры)
    /// </summary>
    private void CreateSystems()
    {
        // GameManager уже на этом объекте или создаём
        if (GameManager.Instance == null)
        {
            gameObject.AddComponent<GameManager>();
        }

        // Checkpoint System
        gameObject.AddComponent<CheckpointSystem>();

        // Audio Manager
        GameObject audioObj = new GameObject("AudioManager");
        audioObj.AddComponent<AudioManager>();

        // Narrator System
        GameObject narratorObj = new GameObject("NarratorSystem");
        narratorObj.AddComponent<NarratorSystem>();

        // Post Processing
        GameObject ppObj = new GameObject("PostProcessing");
        ppObj.AddComponent<PostProcessingController>();

        // UI Manager (создаём после всех систем)
        GameObject uiObj = new GameObject("UIManager");
        UIManager uiMgr = uiObj.AddComponent<UIManager>();
        
        // Инициализируем UI с задержкой чтобы все системы были готовы
        StartCoroutine(InitUIDelayed(uiMgr));
    }

    private System.Collections.IEnumerator InitUIDelayed(UIManager uiMgr)
    {
        yield return null; // Ждём один кадр
        uiMgr.Initialize();
    }

    /// <summary>
    /// Построить выбранную зону
    /// </summary>
    private void BuildZone()
    {
        if (GameManager.Instance != null)
        {
            zoneToLoad = GameManager.Instance.CurrentZone;
        }

        switch (zoneToLoad)
        {
            case 1: gameObject.AddComponent<Zone1Builder>(); break;
            case 2: gameObject.AddComponent<Zone2Builder>(); break;
            case 3: gameObject.AddComponent<Zone3Builder>(); break;
            case 4: gameObject.AddComponent<Zone4Builder>(); break;
            case 5: gameObject.AddComponent<Zone5Builder>(); break;
            case 6: gameObject.AddComponent<Zone6Builder>(); break;
            default: gameObject.AddComponent<Zone1Builder>(); break;
        }
    }

    // === УТИЛИТЫ для ZoneBuilder'ов ===

    /// <summary>
    /// Создать платформу (куб)
    /// </summary>
    public static GameObject CreatePlatform(Vector3 position, Vector3 scale, Material material)
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.transform.position = position;
        platform.transform.localScale = scale;
        platform.GetComponent<Renderer>().material = material;
        platform.isStatic = true;
        return platform;
    }

    /// <summary>
    /// Создать стену
    /// </summary>
    public static GameObject CreateWall(Vector3 position, Vector3 scale)
    {
        return CreatePlatform(position, scale, MaterialFactory.Wall());
    }

    /// <summary>
    /// Создать неоновую полосу (акцент)
    /// </summary>
    public static GameObject CreateNeonStrip(Vector3 position, Vector3 scale)
    {
        return CreatePlatform(position, scale, MaterialFactory.Neon());
    }

    /// <summary>
    /// Создать чекпоинт
    /// </summary>
    public static GameObject CreateCheckpoint(Vector3 position, CheckpointSystem checkpointSystem, int gravityIndex = 0)
    {
        // Визуальный маркер
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        marker.transform.position = position - Vector3.up * 0.4f;
        marker.transform.localScale = new Vector3(2f, 0.1f, 2f);
        marker.GetComponent<Renderer>().material = MaterialFactory.Checkpoint();

        // Триггер
        GameObject trigger = new GameObject("CheckpointTrigger");
        trigger.transform.position = position;
        BoxCollider col = trigger.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(2f, 3f, 2f);

        CheckpointTrigger ct = trigger.AddComponent<CheckpointTrigger>();
        ct.Initialize(checkpointSystem, gravityIndex);

        // Частицы
        ParticleFactory.CreateCheckpointBeam(position);

        return marker;
    }

    /// <summary>
    /// Создать финиш
    /// </summary>
    public static GameObject CreateFinish(Vector3 position)
    {
        GameObject finish = GameObject.CreatePrimitive(PrimitiveType.Cube);
        finish.transform.position = position;
        finish.transform.localScale = new Vector3(3f, 0.1f, 3f);
        finish.GetComponent<Renderer>().material = MaterialFactory.Safe();

        // Используем существующий BoxCollider как триггер
        BoxCollider col = finish.GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = Vector3.one * 1.5f;

        finish.AddComponent<FinishTrigger>();

        return finish;
    }

    /// <summary>
    /// Создать монету
    /// </summary>
    public static GameObject CreateCoin(Vector3 position)
    {
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        coin.transform.position = position;
        coin.transform.localScale = Vector3.one * 0.4f;
        coin.GetComponent<Renderer>().material = MaterialFactory.Collectible();

        // Делаем триггером
        coin.GetComponent<SphereCollider>().isTrigger = true;

        coin.AddComponent<Coin>();
        return coin;
    }
}

/// <summary>
/// Триггер чекпоинта
/// </summary>
public class CheckpointTrigger : MonoBehaviour
{
    private CheckpointSystem checkpointSystem;
    private int gravityIndex;
    private bool activated = false;

    public void Initialize(CheckpointSystem system, int gravIndex)
    {
        checkpointSystem = system;
        gravityIndex = gravIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other.GetComponent<PlayerHealth>() == null) return;

        activated = true;
        if (checkpointSystem != null)
        {
            checkpointSystem.ActivateCheckpoint(transform.position + Vector3.up, other.transform.rotation, gravityIndex);
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCheckpoint();

        if (NarratorSystem.Instance != null)
            NarratorSystem.Instance.ShowMessage("// Прогресс сохранён...");
    }
}

/// <summary>
/// Триггер финиша
/// </summary>
public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() != null)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.PlayerReachedFinish();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayWin();
        }
    }
}
