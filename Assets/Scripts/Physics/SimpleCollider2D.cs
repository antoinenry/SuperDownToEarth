using UnityEngine;

public class SimpleCollider2D : MonoBehaviour
{
    public Trigger onCollisionEnter;
    public Trigger onCollisionExit;
    public BoolChangeEvent onCollisionStay;

    private bool collisionStay;

    public Collision2D CollisionInfos { get; private set; }

    private void FixedUpdate()
    {
        onCollisionStay.Value = collisionStay;
        collisionStay = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionInfos = collision;
        onCollisionEnter.Trigger();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CollisionInfos = collision;
        onCollisionExit.Trigger();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CollisionInfos = collision;
        collisionStay = true;
    }
}
