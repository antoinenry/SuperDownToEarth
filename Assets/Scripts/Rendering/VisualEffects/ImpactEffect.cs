using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    public GameObject impactSurfaceEffect;
    public GameObject leaveSurfaceEffect;

    private void InstanciateCollisionEffect(GameObject effectPrefab, Transform setTransform, bool setParent)
    {
        if (effectPrefab == null) return;
        Instantiate(effectPrefab, setTransform.position, setTransform.rotation, setParent ? setTransform : null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        InstanciateCollisionEffect(impactSurfaceEffect, collision.transform, false);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        InstanciateCollisionEffect(leaveSurfaceEffect, collision.transform, false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        InstanciateCollisionEffect(impactSurfaceEffect, other.transform, false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InstanciateCollisionEffect(impactSurfaceEffect, other.transform, false);
    }
}
