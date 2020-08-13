using UnityEngine;

public class StickToSurface : BodyPart
{
    [Min(0f)] public float stickForce;
    public bool useTrigger;
    public LayerMask ignoreLayers;

    private Rigidbody2D rb2D;
    private Vector2 stickDirection;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (stickDirection != Vector2.zero)
        {
            rb2D.AddForce(stickForce * stickDirection.normalized);
            stickDirection = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ignoreLayers != (ignoreLayers | (1 << collision.gameObject.layer)))
            stickDirection = (collision.GetContact(0).point - rb2D.position);        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (useTrigger)
        {
            if (ignoreLayers != (ignoreLayers | (1 << collision.gameObject.layer)))            
                stickDirection = (collision.ClosestPoint(rb2D.position) - rb2D.position);
        }
    }
}
