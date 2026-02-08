using UnityEngine;

public class ParticleLifetime : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        if (ps != null)
        {
            float lifetime = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(gameObject, lifetime);
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }
}
