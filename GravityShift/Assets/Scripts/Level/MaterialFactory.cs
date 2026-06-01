using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Фабрика материалов.
/// Создаёт и кеширует материалы для всех объектов уровня.
/// Sci-fi стиль: белые стены, неоновые акценты, свечение.
/// </summary>
public static class MaterialFactory
{
    private static Dictionary<string, Material> cache = new Dictionary<string, Material>();

    /// <summary>
    /// Получить базовый шейдер
    /// </summary>
    private static Shader GetShader()
    {
        // Пробуем URP шейдер, если нет — стандартный, если нет — Diffuse (гарантированно есть)
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Diffuse");
        return shader;
    }

    /// <summary>
    /// Создать материал с emission (свечение)
    /// </summary>
    private static Material CreateEmissive(string name, Color baseColor, Color emissionColor, float emissionIntensity = 2f)
    {
        if (cache.ContainsKey(name)) return cache[name];

        Material mat = new Material(GetShader());
        mat.name = name;
        mat.color = baseColor;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        cache[name] = mat;
        return mat;
    }

    /// <summary>
    /// Создать обычный непрозрачный материал
    /// </summary>
    private static Material CreateSolid(string name, Color color)
    {
        if (cache.ContainsKey(name)) return cache[name];

        Material mat = new Material(GetShader());
        mat.name = name;
        mat.color = color;

        cache[name] = mat;
        return mat;
    }

    /// <summary>
    /// Создать полупрозрачный материал
    /// </summary>
    private static Material CreateTransparent(string name, Color color)
    {
        if (cache.ContainsKey(name)) return cache[name];

        Material mat = new Material(GetShader());
        mat.name = name;

        // Настройки прозрачности
        mat.SetFloat("_Surface", 1); // Transparent
        mat.SetFloat("_Blend", 0); // Alpha
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.renderQueue = 3000;
        mat.color = color;

        cache[name] = mat;
        return mat;
    }

    // === МАТЕРИАЛЫ ДЛЯ УРОВНЯ ===

    /// <summary>
    /// Белые/серые стены лаборатории
    /// </summary>
    public static Material Wall()
    {
        return CreateSolid("Wall", new Color(0.9f, 0.9f, 0.92f));
    }

    /// <summary>
    /// Тёмный пол
    /// </summary>
    public static Material Floor()
    {
        return CreateSolid("Floor", new Color(0.2f, 0.2f, 0.25f));
    }

    /// <summary>
    /// Неоновые синие акцентные линии
    /// </summary>
    public static Material Neon()
    {
        return CreateEmissive("Neon", new Color(0.1f, 0.3f, 1f), new Color(0.2f, 0.5f, 1f), 3f);
    }

    /// <summary>
    /// Красное опасное препятствие (пульсирующее свечение)
    /// </summary>
    public static Material Danger()
    {
        return CreateEmissive("Danger", new Color(0.8f, 0.1f, 0.1f), new Color(1f, 0.2f, 0.1f), 2f);
    }

    /// <summary>
    /// Зелёная безопасная зона / финиш
    /// </summary>
    public static Material Safe()
    {
        return CreateEmissive("Safe", new Color(0.1f, 0.8f, 0.2f), new Color(0.2f, 1f, 0.3f), 2f);
    }

    /// <summary>
    /// Синий чекпоинт
    /// </summary>
    public static Material Checkpoint()
    {
        return CreateEmissive("Checkpoint", new Color(0.1f, 0.5f, 1f), new Color(0.2f, 0.6f, 1f), 2.5f);
    }

    /// <summary>
    /// Жёлтая падающая платформа
    /// </summary>
    public static Material Falling()
    {
        return CreateEmissive("Falling", new Color(1f, 0.8f, 0.1f), new Color(1f, 0.9f, 0.2f), 1.5f);
    }

    /// <summary>
    /// Оранжевая толкающая стена
    /// </summary>
    public static Material Pusher()
    {
        return CreateEmissive("Pusher", new Color(1f, 0.5f, 0.1f), new Color(1f, 0.6f, 0.1f), 2f);
    }

    /// <summary>
    /// Голубой коллекционный предмет
    /// </summary>
    public static Material Collectible()
    {
        return CreateEmissive("Collectible", new Color(0.1f, 0.9f, 1f), new Color(0.2f, 1f, 1f), 3f);
    }

    /// <summary>
    /// Фиолетовая гравитационная зона
    /// </summary>
    public static Material GravityZoneMat()
    {
        return CreateTransparent("GravityZone", new Color(0.6f, 0.1f, 1f, 0.3f));
    }

    /// <summary>
    /// Стеклянный/прозрачный материал
    /// </summary>
    public static Material Glass()
    {
        return CreateTransparent("Glass", new Color(0.8f, 0.9f, 1f, 0.2f));
    }

    /// <summary>
    /// Материал конвейера
    /// </summary>
    public static Material Conveyor()
    {
        return CreateEmissive("Conveyor", new Color(0.3f, 0.3f, 0.3f), new Color(0.5f, 0.5f, 0.1f), 1f);
    }

    /// <summary>
    /// Лазерный материал
    /// </summary>
    public static Material Laser()
    {
        return CreateEmissive("Laser", new Color(1f, 0f, 0f), new Color(1f, 0f, 0f), 5f);
    }

    /// <summary>
    /// Щит (синий полупрозрачный)
    /// </summary>
    public static Material ShieldMat()
    {
        return CreateTransparent("Shield", new Color(0.2f, 0.5f, 1f, 0.5f));
    }

    /// <summary>
    /// Очистить кеш (при перезагрузке сцены)
    /// </summary>
    public static void ClearCache()
    {
        cache.Clear();
    }
}
