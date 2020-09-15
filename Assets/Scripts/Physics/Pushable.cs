using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pushable : MonoBehaviour
{
    public float velocityThreshold = 15f;
    public float massThreshold = .1f;
    [Range(0f, 1f)] public float velocityTransferRatio = .5f;

    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D incoming = collision.rigidbody;
        if (incoming != null)
        {
            if (incoming.mass >= massThreshold && Vector2.Dot(collision.relativeVelocity, collision.GetContact(0).normal) >= velocityThreshold)
            {
                rb2D.constraints = RigidbodyConstraints2D.None;
                rb2D.velocity = velocityTransferRatio * collision.relativeVelocity;
            }
        }
    }
}
