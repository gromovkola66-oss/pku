using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Менеджер интерфейса.
/// Создаёт весь UI (Canvas, HUD, меню) программно при запуске.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // UI панели
    private GameObject hudPanel;
    private GameObject pausePanel;
    private GameObject deathPanel;
    private GameObject winPanel;
    private GameObject narratorPanel;
    private GameObject mainMenuPanel;

    // HUD элементы
    private Text timerText;
    private Text coinsText;
    private Text zoneText;
    private Text healthText;
    private Text cooldownText;
    private Text gravityArrowText;

    // Narrator
    private Text narratorText;

    // Win screen
    private Text winTimeText;
    private Text winStarsText;

    private Canvas canvas;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Создать весь UI
    /// </summary>
    public void Initialize()
    {
        CreateCanvas();
        CreateHUD();
        CreatePauseMenu();
        CreateDeathScreen();
        CreateWinScreen();
        CreateNarratorPanel();
        CreateEffectOverlays();

        // Подписываемся на события
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused += ShowPauseMenu;
            GameManager.Instance.OnGameResumed += HidePauseMenu;
            GameManager.Instance.OnGameOver += ShowDeathScreen;
            GameManager.Instance.OnGameWon += ShowWinScreen;
        }
    }

    // Кешированные ссылки для Update (чтобы не вызывать FindObjectOfType каждый кадр)
    private PlayerHealth cachedHealth;
    private PlayerInventory cachedInventory;
    private GravityController cachedGravity;
    private bool cacheInitialized = false;

    private void Update()
    {
        if (!cacheInitialized)
        {
            cachedHealth = FindObjectOfType<PlayerHealth>();
            cachedInventory = FindObjectOfType<PlayerInventory>();
            cachedGravity = FindObjectOfType<GravityController>();
            if (cachedHealth != null) cacheInitialized = true;
        }
        UpdateHUD();
    }

    /// <summary>
    /// Создать Canvas
    /// </summary>
    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("UICanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    /// <summary>
    /// Создать HUD
    /// </summary>
    private void CreateHUD()
    {
        hudPanel = CreatePanel("HUD", new Color(0, 0, 0, 0));

        // Таймер (верхний центр)
        timerText = CreateText(hudPanel, "Timer", "00:00.00",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -10), 24);

        // Монеты (верхний левый)
        coinsText = CreateText(hudPanel, "Coins", "Монеты: 0",
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20, -10), 20);

        // Зона (верхний правый)
        zoneText = CreateText(hudPanel, "Zone", "Зона 1",
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20, -10), 20);

        // Здоровье (нижний левый)
        healthText = CreateText(hudPanel, "Health", "HP: ♥♥♥",
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(20, 40), 22);

        // Кулдаун гравитации (нижний центр)
        cooldownText = CreateText(hudPanel, "Cooldown", "◆ Гравитация",
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 40), 18);

        // Индикатор направления (правый центр)
        gravityArrowText = CreateText(hudPanel, "GravityArrow", "↓",
            new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-40, 0), 40);

        // Прицел (центр)
        Text crosshair = CreateText(hudPanel, "Crosshair", "+",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), 20);
        crosshair.color = new Color(1, 1, 1, 0.5f);
    }

    /// <summary>
    /// Обновление HUD каждый кадр
    /// </summary>
    private void UpdateHUD()
    {
        if (GameManager.Instance == null) return;

        // Таймер
        float time = GameManager.Instance.GameTime;
        int mins = (int)(time / 60f);
        float secs = time % 60f;
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00.00}", mins, secs);

        // Зона
        if (zoneText != null)
            zoneText.text = "Зона " + GameManager.Instance.CurrentZone;

        // Здоровье
        if (cachedHealth != null && healthText != null)
        {
            string hearts = "";
            for (int i = 0; i < cachedHealth.MaxHealth; i++)
            {
                hearts += i < cachedHealth.CurrentHealth ? "♥" : "♡";
            }
            if (cachedHealth.HasShield) hearts += " [S]";
            healthText.text = hearts;
        }

        // Монеты
        if (cachedInventory != null && coinsText != null)
            coinsText.text = "Монеты: " + cachedInventory.Coins;

        // Гравитация
        if (cachedGravity != null)
        {
            // Стрелка направления
            Vector3 dir = cachedGravity.GetGravityDirection();
            string arrow = "v";
            if (dir == Vector3.up) arrow = "^";
            else if (dir == Vector3.left) arrow = "<";
            else if (dir == Vector3.right) arrow = ">";
            if (gravityArrowText != null) gravityArrowText.text = arrow;

            // Кулдаун
            float cd = cachedGravity.GetCooldownProgress();
            if (cooldownText != null)
            {
                if (cd >= 1f)
                    cooldownText.text = "[OK] Q/E";
                else
                    cooldownText.text = string.Format("[..] {0:0}%", cd * 100);
            }
        }
    }

    /// <summary>
    /// Меню паузы
    /// </summary>
    private void CreatePauseMenu()
    {
        pausePanel = CreatePanel("PauseMenu", new Color(0, 0, 0, 0.8f));
        pausePanel.SetActive(false);

        CreateText(pausePanel, "Title", "ПАУЗА",
            new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero, 48);
        CreateText(pausePanel, "Info", "ESC - продолжить\nR - начать заново",
            new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.4f), Vector2.zero, 24);
    }

    /// <summary>
    /// Экран смерти
    /// </summary>
    private void CreateDeathScreen()
    {
        deathPanel = CreatePanel("DeathScreen", new Color(0.3f, 0, 0, 0.85f));
        deathPanel.SetActive(false);

        CreateText(deathPanel, "Title", "ЭКСПЕРИМЕНТ ПРОВАЛЕН",
            new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), Vector2.zero, 42);
        CreateText(deathPanel, "Info", "Нажми R чтобы попробовать снова",
            new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.4f), Vector2.zero, 24);
    }

    /// <summary>
    /// Экран победы
    /// </summary>
    private void CreateWinScreen()
    {
        winPanel = CreatePanel("WinScreen", new Color(0, 0.1f, 0, 0.85f));
        winPanel.SetActive(false);

        CreateText(winPanel, "Title", "ЭКСПЕРИМЕНТ ЗАВЕРШЁН",
            new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero, 42);
        winTimeText = CreateText(winPanel, "Time", "Время: 00:00",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 28);
        winStarsText = CreateText(winPanel, "Stars", "★★★",
            new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.4f), Vector2.zero, 36);
        CreateText(winPanel, "Info", "R - начать заново",
            new Vector2(0.5f, 0.25f), new Vector2(0.5f, 0.25f), Vector2.zero, 22);
    }

    /// <summary>
    /// Панель нарратора (нижняя часть экрана)
    /// </summary>
    private void CreateNarratorPanel()
    {
        narratorPanel = CreatePanel("NarratorPanel", new Color(0, 0, 0, 0.7f), 
            new Vector2(0.1f, 0.02f), new Vector2(0.9f, 0.12f));
        
        narratorText = CreateText(narratorPanel, "NarratorText", "",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 20);
        narratorText.color = new Color(0.7f, 0.9f, 1f);

        narratorPanel.SetActive(false);

        // Привязываем к системе нарратора
        if (NarratorSystem.Instance != null)
        {
            NarratorSystem.Instance.SetUI(narratorText, narratorPanel);
        }
    }

    /// <summary>
    /// Оверлеи для эффектов (виньетка, вспышки)
    /// </summary>
    private void CreateEffectOverlays()
    {
        // Виньетка урона
        GameObject vigObj = new GameObject("DamageVignette");
        vigObj.transform.SetParent(canvas.transform);
        RectTransform vigRect = vigObj.AddComponent<RectTransform>();
        vigRect.anchorMin = Vector2.zero;
        vigRect.anchorMax = Vector2.one;
        vigRect.offsetMin = Vector2.zero;
        vigRect.offsetMax = Vector2.zero;
        Image vigImage = vigObj.AddComponent<Image>();
        vigImage.color = new Color(1, 0, 0, 0);
        vigImage.raycastTarget = false;

        // Вспышка
        GameObject flashObj = new GameObject("Flash");
        flashObj.transform.SetParent(canvas.transform);
        RectTransform flashRect = flashObj.AddComponent<RectTransform>();
        flashRect.anchorMin = Vector2.zero;
        flashRect.anchorMax = Vector2.one;
        flashRect.offsetMin = Vector2.zero;
        flashRect.offsetMax = Vector2.zero;
        Image flashImage = flashObj.AddComponent<Image>();
        flashImage.color = new Color(0, 0, 0, 0);
        flashImage.raycastTarget = false;

        // Привязываем к PostProcessingController
        if (PostProcessingController.Instance != null)
        {
            PostProcessingController.Instance.SetEffectImages(vigImage, flashImage);
        }
    }

    // === UI хелперы ===

    private void ShowPauseMenu() { if (pausePanel) pausePanel.SetActive(true); }
    private void HidePauseMenu() { if (pausePanel) pausePanel.SetActive(false); }
    private void ShowDeathScreen() { if (deathPanel) deathPanel.SetActive(true); }
    private void ShowWinScreen()
    {
        if (winPanel == null) return;
        winPanel.SetActive(true);
        if (winTimeText != null)
        {
            float t = GameManager.Instance.GameTime;
            winTimeText.text = string.Format("Время: {0:00}:{1:00.00}", (int)(t/60), t%60);
        }
        if (winStarsText != null)
        {
            int stars = GameManager.Instance.CalculateStars(GameManager.Instance.GameTime, 0, 0);
            winStarsText.text = new string('★', stars) + new string('☆', 3 - stars);
        }
    }

    /// <summary>
    /// Создать панель
    /// </summary>
    private GameObject CreatePanel(string name, Color bgColor, Vector2? anchorMin = null, Vector2? anchorMax = null)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(canvas.transform);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin ?? Vector2.zero;
        rect.anchorMax = anchorMax ?? Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = bgColor;
        img.raycastTarget = (bgColor.a > 0.5f);

        return panel;
    }

    /// <summary>
    /// Создать текст
    /// </summary>
    private Text CreateText(GameObject parent, string name, string content,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offset, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = anchorMin;
        rect.anchoredPosition = offset;
        rect.sizeDelta = new Vector2(400, fontSize + 20);

        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        // Пытаемся найти шрифт, с fallback на встроенный
        Font font = Font.CreateDynamicFontFromOSFont("Arial", fontSize);
        if (font == null) font = Font.CreateDynamicFontFromOSFont("Liberation Sans", fontSize);
        if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.font = font;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        return text;
    }
}
