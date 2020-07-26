using UnityEngine;

public class ImmersionEffectAgent : MonoBehaviour
{
    public GameObject effectInstance;
    public Collider2D immersionCollider;
    public float destroyDelay;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == immersionCollider)
        {
            Destroy(effectInstance, destroyDelay);
            Destroy(this, destroyDelay);
        }
    }
}
