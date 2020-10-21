using UnityEngine;

public class LocalGravity : BodyPart
{
    public float gravityForce;
    [Min(0f)] public float fallVelocityThreshold;

    public BoolChangeEvent isFalling;
    public FloatChangeEvent fallSpeed;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
    }

    private void OnDisable()
    {
        isFalling.Value = false;
    }

    private void FixedUpdate ()
    {
        Vector2 localDown = transform.rotation * Vector2.down;

        AttachedRigidbody.AddForce(localDown * gravityForce);

        fallSpeed.Value = Mathf.Max(Vector2.Dot(AttachedRigidbody.velocity, localDown), 0f);
        isFalling.Value = (fallSpeed > fallVelocityThreshold);
    }    
}