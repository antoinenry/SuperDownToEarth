using UnityEngine;

public abstract class BodyPart : MonoBehaviour
{
    private Body body;

    public Body AttachedBody
    {
        get => body;

        set
        {
            body = value;
            if (body != null)
                AttachedRigidbody = body.AttachedRigidBody;
            else
                AttachedRigidbody = null;
        }
    }

    public Rigidbody2D AttachedRigidbody { get; private set; }
}
