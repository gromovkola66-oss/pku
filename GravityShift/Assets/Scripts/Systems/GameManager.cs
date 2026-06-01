using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Главный менеджер игры (Singleton).
/// Управляет состоянием игры, зонами, паузой, рестартом.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Состояние игры")]
    [SerializeField] private int currentZone = 1;
    [SerializeField] private float gameTime = 0f;
    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isGameOver = false;
    [SerializeField] private bool isGameWon = false;

    [Header("Настройки")]
    [Tooltip("Высота, ниже которой игрок умирает")]
    [SerializeField] private float deathHeight = -30f;

    // События
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action OnGameOver;
    public event Action OnGameWon;
    public event Action<int> OnZoneChanged;

    // Свойства
    public int CurrentZone => currentZone;
    public float GameTime => gameTime;
    public bool IsPaused => isPaused;
    public bool IsGameOver => isGameOver;

    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private CheckpointSystem checkpointSystem;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Находим игрока (будет создан LevelBuilder)
        StartCoroutine(FindPlayerDelayed());
    }

    private System.Collections.IEnumerator FindPlayerDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath += HandlePlayerDeath;
            }
        }
        checkpointSystem = FindObjectOfType<CheckpointSystem>();
    }

    private void Update()
    {
        if (isGameOver || isGameWon) return;

        // Таймер
        if (!isPaused)
        {
            gameTime += Time.deltaTime;
        }

        // Пауза
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Рестарт
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        // Проверка падения
        if (playerTransform != null && playerTransform.position.y < deathHeight)
        {
            HandlePlayerFall();
        }
    }

    /// <summary>
    /// Переключить паузу
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnGamePaused?.Invoke();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnGameResumed?.Invoke();
        }
    }

    /// <summary>
    /// Рестарт текущей зоны
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        isGameWon = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Обработка смерти игрока
    /// </summary>
    private void HandlePlayerDeath()
    {
        isGameOver = true;
        OnGameOver?.Invoke();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Обработка падения игрока
    /// </summary>
    private void HandlePlayerFall()
    {
        if (checkpointSystem != null)
        {
            // Респаун на чекпоинте
            var data = checkpointSystem.GetRespawnData();
            playerTransform.position = data.position;
            playerTransform.rotation = data.rotation;

            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null) rb.velocity = Vector3.zero;

            GravityController gc = playerTransform.GetComponent<GravityController>();
            if (gc != null) gc.ForceGravityDirection(data.gravityIndex);

            if (playerHealth != null) playerHealth.TakeDamage(1);
        }
        else
        {
            if (playerHealth != null) playerHealth.TakeDamage(3); // Мгновенная смерть
        }
    }

    /// <summary>
    /// Игрок достиг финиша
    /// </summary>
    public void PlayerReachedFinish()
    {
        if (isGameWon) return;
        isGameWon = true;

        // Сохраняем лучшее время
        string key = "BestTime_Zone" + currentZone;
        float bestTime = PlayerPrefs.GetFloat(key, float.MaxValue);
        if (gameTime < bestTime)
        {
            PlayerPrefs.SetFloat(key, gameTime);
            PlayerPrefs.Save();
        }

        OnGameWon?.Invoke();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Перейти к следующей зоне
    /// </summary>
    public void NextZone()
    {
        currentZone++;
        if (currentZone > 6) currentZone = 1;
        OnZoneChanged?.Invoke(currentZone);
        RestartGame();
    }

    /// <summary>
    /// Установить текущую зону
    /// </summary>
    public void SetZone(int zone)
    {
        currentZone = Mathf.Clamp(zone, 1, 6);
        OnZoneChanged?.Invoke(currentZone);
    }

    /// <summary>
    /// Рассчитать звёзды (1-3) на основе времени
    /// </summary>
    public int CalculateStars(float time, int coinsCollected, int totalCoins)
    {
        int stars = 1; // Прошёл = 1 звезда

        // Целевое время для 2 звёзд (зависит от зоны)
        float targetTime = currentZone * 60f; // 1 минута на зону
        if (time <= targetTime) stars = 2;

        // 3 звезды = уложился во время + все монеты
        if (time <= targetTime && coinsCollected >= totalCoins) stars = 3;

        return stars;
    }
}
