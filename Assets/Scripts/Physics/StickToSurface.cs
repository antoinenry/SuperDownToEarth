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
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector2 stickDirection = (collision.GetContact(0).point - rb2D.position);

        if(stickDirection != Vector2.zero)
        {
            rb2D.AddForce(stickForce * stickDirection.normalized);
        }
    }
}
