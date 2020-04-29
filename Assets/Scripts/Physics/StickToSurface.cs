using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToSurface : MonoBehaviour
{
    [Min(0f)] public float stickForce;

    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (rb2D == null || collision.isTrigger) return;

        Vector2 stickDirection = (collision.ClosestPoint(rb2D.position) - rb2D.position);

        if(stickDirection != Vector2.zero)
        {
            rb2D.AddForce(stickForce * stickDirection.normalized);
        }
    }
}
