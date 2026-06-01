using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Система нарратора (ИИ-голос лаборатории).
/// Показывает текст с эффектом печатной машинки.
/// Сообщения на русском языке.
/// </summary>
public class NarratorSystem : MonoBehaviour
{
    public static NarratorSystem Instance { get; private set; }

    [Header("Настройки")]
    [Tooltip("Скорость печати (символов/сек)")]
    [SerializeField] private float typeSpeed = 30f;

    [Tooltip("Время показа сообщения после печати (сек)")]
    [SerializeField] private float displayTime = 3f;

    [Tooltip("Уровень 'глитча' текста (0 = нет, 1 = максимум)")]
    [SerializeField] private float glitchLevel = 0f;

    // UI элементы (создаются UIManager)
    private Text narratorText;
    private GameObject narratorPanel;

    // Очередь сообщений
    private Queue<string> messageQueue = new Queue<string>();
    private bool isDisplaying = false;
    private string currentFullText = "";
    private int currentCharIndex = 0;

    // Глитч-символы для поздних уровней
    private string glitchChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Привязать UI элементы
    /// </summary>
    public void SetUI(Text text, GameObject panel)
    {
        narratorText = text;
        narratorPanel = panel;
        if (narratorPanel != null)
            narratorPanel.SetActive(false);
    }

    /// <summary>
    /// Показать сообщение нарратора
    /// </summary>
    public void ShowMessage(string message)
    {
        messageQueue.Enqueue(message);
        if (!isDisplaying)
        {
            StartCoroutine(DisplayNextMessage());
        }
    }

    /// <summary>
    /// Установить уровень глитча (для поздних зон)
    /// </summary>
    public void SetGlitchLevel(float level)
    {
        glitchLevel = Mathf.Clamp01(level);
    }

    /// <summary>
    /// Корутина отображения сообщения с эффектом печати
    /// </summary>
    private IEnumerator DisplayNextMessage()
    {
        isDisplaying = true;

        while (messageQueue.Count > 0)
        {
            currentFullText = messageQueue.Dequeue();
            currentCharIndex = 0;

            // Показываем панель
            if (narratorPanel != null)
                narratorPanel.SetActive(true);

            if (narratorText != null)
                narratorText.text = "";

            // Эффект печатной машинки
            while (currentCharIndex < currentFullText.Length)
            {
                currentCharIndex++;
                string displayText = currentFullText.Substring(0, currentCharIndex);

                // Применяем глитч к последнему символу
                if (glitchLevel > 0 && Random.value < glitchLevel * 0.3f)
                {
                    char glitchChar = glitchChars[Random.Range(0, glitchChars.Length)];
                    displayText = displayText.Substring(0, displayText.Length - 1) + glitchChar;
                }

                if (narratorText != null)
                    narratorText.text = displayText;

                yield return new WaitForSecondsRealtime(1f / typeSpeed);
            }

            // Показываем финальный текст без глитча
            if (narratorText != null)
                narratorText.text = currentFullText;

            // Ждём перед скрытием
            yield return new WaitForSecondsRealtime(displayTime);
        }

        // Скрываем панель
        if (narratorPanel != null)
            narratorPanel.SetActive(false);

        isDisplaying = false;
    }
}
