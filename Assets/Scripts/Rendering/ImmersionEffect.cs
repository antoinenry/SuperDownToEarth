using UnityEngine;

public class ImmersionEffect : MonoBehaviour
{
    public GameObject effectPrefab;
    public bool limitParticlesToCollider;
    public float effectDestructionDelay = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (effectPrefab == null) return;
        
        ImmersionEffectAgent effectAgent = other.gameObject.AddComponent<ImmersionEffectAgent>();
        effectAgent.effectInstance = Instantiate(effectPrefab, other.transform);
        effectAgent.immersionCollider = GetComponent<Collider2D>();
        effectAgent.destroyDelay = effectDestructionDelay;

        if (limitParticlesToCollider)
        {            
            ParticleSystem[] particles = effectAgent.effectInstance.GetComponentsInChildren<ParticleSystem>(true);
            foreach(ParticleSystem ps in particles)
            {
                if (ps.trigger.maxColliderCount > 0)
                    ps.trigger.SetCollider(0, effectAgent.immersionCollider);
            }
        }
    }
}
