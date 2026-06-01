using UnityEngine;

/// <summary>
/// Менеджер звука (заглушка).
/// Структура готова для добавления аудио файлов.
/// Перетащи .mp3/.wav файлы в соответствующие поля в Inspector.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Звуки игрока")]
    [Tooltip("Звук прыжка")]
    [SerializeField] private AudioClip jumpSound;

    [Tooltip("Звук приземления")]
    [SerializeField] private AudioClip landSound;

    [Tooltip("Звук получения урона")]
    [SerializeField] private AudioClip damageSound;

    [Tooltip("Звук смерти")]
    [SerializeField] private AudioClip deathSound;

    [Header("Звуки переключения гравитации")]
    [Tooltip("Звук переключения гравитации")]
    [SerializeField] private AudioClip gravitySwitchSound;

    [Header("Звуки предметов")]
    [Tooltip("Звук подбора монеты")]
    [SerializeField] private AudioClip coinSound;

    [Tooltip("Звук подбора бонуса")]
    [SerializeField] private AudioClip powerupSound;

    [Header("Звуки интерфейса")]
    [Tooltip("Звук победы")]
    [SerializeField] private AudioClip winSound;

    [Tooltip("Звук чекпоинта")]
    [SerializeField] private AudioClip checkpointSound;

    [Header("Музыка")]
    [Tooltip("Фоновая музыка")]
    [SerializeField] private AudioClip backgroundMusic;

    // Аудио компоненты
    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Создаём аудио источники
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = 0.3f;
    }

    private void Start()
    {
        // Запускаем фоновую музыку если есть
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Проиграть звуковой эффект (безопасно — не крашится без аудио файла)
    /// </summary>
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayLand() => PlaySFX(landSound);
    public void PlayDamage() => PlaySFX(damageSound);
    public void PlayDeath() => PlaySFX(deathSound);
    public void PlayGravitySwitch() => PlaySFX(gravitySwitchSound);
    public void PlayCoin() => PlaySFX(coinSound);
    public void PlayPowerup() => PlaySFX(powerupSound);
    public void PlayWin() => PlaySFX(winSound);
    public void PlayCheckpoint() => PlaySFX(checkpointSound);

    /// <summary>
    /// Установить громкость музыки (0-1)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Установить громкость эффектов (0-1)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp01(volume);
    }
}
