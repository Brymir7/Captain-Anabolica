using UnityEngine;

public class ParticleCleanup : MonoBehaviour
{
    private ParticleSystem[] _childParticleSystems;

    void Start()
    {
        _childParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        bool allParticlesFinished = true;
        foreach (var ps in _childParticleSystems)
        {
            if (ps.IsAlive())
            {
                allParticlesFinished = false;
                break;
            }
        }
        if (allParticlesFinished)
        {
            foreach (var ps in _childParticleSystems)
            {
                Destroy(ps.gameObject);
            }
            Destroy(gameObject);
        }
    }
}