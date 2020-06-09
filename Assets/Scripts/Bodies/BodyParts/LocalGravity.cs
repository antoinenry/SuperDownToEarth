using UnityEngine;

public class LocalGravity : BodyPart
{
    public float gravityForce;
    public float angleOffset;

    public BoolChangeEvent isFalling;

    public float FallSpeed { get; private set; }

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
    }

    private void FixedUpdate ()
    {
        Vector2 localDown = Quaternion.Euler(0f, 0f, angleOffset) * transform.rotation * Vector2.down;

        AttachedRigidbody.AddForce(localDown * gravityForce);

        FallSpeed = Vector2.Dot(AttachedRigidbody.velocity, localDown);
        isFalling.Value = (FallSpeed > 0f);
    }
}