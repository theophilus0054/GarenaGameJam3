using UnityEngine;
using System.Collections.Generic;

public enum ParticleType
{
    Achieve,
    Spark,
    Wiggle,
    Appear
}

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [Header("Particle Prefabs")]
    public GameObject achieveParticle;
    public GameObject sparkParticle;
    public GameObject wiggleParticle;
    public GameObject appearParticle;

    private Dictionary<ParticleType, GameObject> particleDict;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        particleDict = new Dictionary<ParticleType, GameObject>()
        {
            { ParticleType.Achieve, achieveParticle },
            { ParticleType.Spark, sparkParticle },
            { ParticleType.Wiggle, wiggleParticle },
            { ParticleType.Appear, appearParticle }
        };
    }

    // ================= SPAWN AT TRANSFORM =================
    public void Spawn(ParticleType type, Transform target)
    {
        Spawn(type, target.position);
    }

    // ================= SPAWN AT POSITION =================
    public void Spawn(ParticleType type, Vector3 position)
    {
        if (!particleDict.TryGetValue(type, out GameObject prefab) || prefab == null)
        {
            Debug.LogWarning("Particle not assigned: " + type);
            return;
        }

        Instantiate(prefab, position, Quaternion.identity);
    }

    // ================= SPAWN + SOUND =================
    public void SpawnWithSound(ParticleType type, Transform target)
    {
        Spawn(type, target.position);

        switch (type)
        {
            case ParticleType.Achieve:
                SoundManager.PlaySound(SoundType.SparklingDone);
                break;

            case ParticleType.Spark:
                SoundManager.PlaySound(SoundType.LightningSpark);
                break;

            case ParticleType.Wiggle:
                SoundManager.PlaySound(SoundType.BottleWiggle);
                break;

            case ParticleType.Appear:
                SoundManager.PlaySound(SoundType.Poof);
                break;
        }
    }
}
