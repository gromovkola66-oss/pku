using UnityEngine;

/// <summary>
/// Фабрика частиц.
/// Создаёт системы частиц для различных эффектов.
/// </summary>
public static class ParticleFactory
{
    /// <summary>
    /// Получить материал для частиц (с fallback)
    /// </summary>
    private static Material GetParticleMaterial(Color color)
    {
        Shader shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.color = color;
        return mat;
    }

    /// <summary>
    /// Создать частицы пыли, летящие в направлении гравитации
    /// </summary>
    public static ParticleSystem CreateGravityDust(Transform parent)
    {
        GameObject obj = new GameObject("GravityDust");
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = 1f;
        main.startSize = 0.05f;
        main.maxParticles = 100;
        main.startColor = new Color(0.7f, 0.8f, 1f, 0.5f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 20f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(10f, 10f, 10f);

        var renderer = obj.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetParticleMaterial(new Color(0.7f, 0.8f, 1f, 0.3f));

        return ps;
    }

    /// <summary>
    /// Создать волну при переключении гравитации
    /// </summary>
    public static void CreateSwitchWave(Vector3 position)
    {
        GameObject obj = new GameObject("SwitchWave");
        obj.transform.position = position;

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 10f;
        main.startSize = 0.2f;
        main.maxParticles = 50;
        main.startColor = new Color(0.3f, 0.6f, 1f, 0.8f);
        main.loop = false;
        main.duration = 0.2f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 30)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        var renderer = obj.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetParticleMaterial(new Color(0.3f, 0.6f, 1f, 0.8f));

        ps.Play();
        Object.Destroy(obj, 1f);
    }

    /// <summary>
    /// Создать пыль при приземлении
    /// </summary>
    public static void CreateLandingDust(Vector3 position)
    {
        GameObject obj = new GameObject("LandingDust");
        obj.transform.position = position;

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.8f;
        main.startSpeed = 3f;
        main.startSize = 0.15f;
        main.maxParticles = 20;
        main.startColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
        main.loop = false;
        main.duration = 0.1f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 15)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.5f;

        var renderer = obj.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetParticleMaterial(new Color(0.7f, 0.7f, 0.7f, 0.4f));

        ps.Play();
        Object.Destroy(obj, 2f);
    }

    /// <summary>
    /// Создать красные искры при уроне
    /// </summary>
    public static void CreateDamageParticles(Vector3 position)
    {
        GameObject obj = new GameObject("DamageParticles");
        obj.transform.position = position;

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.6f;
        main.startSpeed = 8f;
        main.startSize = 0.1f;
        main.maxParticles = 30;
        main.startColor = new Color(1f, 0.2f, 0.1f, 1f);
        main.loop = false;
        main.duration = 0.1f;
        main.gravityModifier = 2f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 20)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;

        var renderer = obj.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetParticleMaterial(new Color(1f, 0.3f, 0.1f, 1f));

        ps.Play();
        Object.Destroy(obj, 2f);
    }

    /// <summary>
    /// Создать всплеск при подборе предмета
    /// </summary>
    public static void CreateCollectBurst(Vector3 position, Color color)
    {
        GameObject obj = new GameObject("CollectBurst");
        obj.transform.position = position;

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.8f;
        main.startSpeed = 5f;
        main.startSize = 0.15f;
        main.maxParticles = 25;
        main.startColor = color;
        main.loop = false;
        main.duration = 0.1f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 20)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var renderer = obj.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetParticleMaterial(color);

        ps.Play();
        Object.Destroy(obj, 2f);
    }

    /// <summary>
    /// Создать луч чекпоинта
    /// </summary>
    public static GameObject CreateCheckpointBeam(Vector3 position)
    {
        GameObject obj = new GameObject("CheckpointBeam");
        obj.transform.position = position;

        ParticleSystem ps = obj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = 3f;
        main.startSize = 0.1f;
        main.maxParticles = 50;
        main.startColor = new Color(0.2f, 0.6f, 1f, 0.7f);
        main.loop = true;

        var emission = ps.emission;
        emission.rateOverTime = 15f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 5f;
        shape.radius = 0.3f;

        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.y = 2f;

        var renderer = obj.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetParticleMaterial(new Color(0.3f, 0.7f, 1f, 0.6f));

        return obj;
    }
}
