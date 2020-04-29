using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    public GameObject impactPrefab;
    public GameObject tafeOffPrefab;

    private void InstanciateCollisionEffect(GameObject effectPrefab, Collision2D collision)
    {
        if (effectPrefab == null) return;

        Vector3 splooshPosition = collision.transform.position;
        Instantiate<GameObject>(effectPrefab, splooshPosition, Quaternion.Euler(0f, 0f, collision.rigidbody.rotation));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        InstanciateCollisionEffect(impactPrefab, collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        InstanciateCollisionEffect(tafeOffPrefab, collision);
    }
}
