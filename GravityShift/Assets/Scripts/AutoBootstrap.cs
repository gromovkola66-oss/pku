using UnityEngine;

/// <summary>
/// Автоматический запуск игры.
/// Этот скрипт запускается через атрибут [RuntimeInitializeOnLoadMethod]
/// и не требует привязки к объекту на сцене.
/// </summary>
public static class AutoBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        // Проверяем, нет ли уже LevelBuilder
        if (Object.FindFirstObjectByType<LevelBuilder>() != null) return;

        // Создаём GameBootstrapper программно
        GameObject bootstrapper = new GameObject("GameBootstrapper_Auto");
        bootstrapper.AddComponent<LevelBuilder>();
    }
}
