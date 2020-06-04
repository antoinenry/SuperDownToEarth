using UnityEngine;

public class Spinner : BodyPart
{
    public bool invertDirection = true;
    public float spinVelocity;
    public float spinInertia;
    public float tumbleRecuperation;
    
    public Feet Feet { get; private set; }

    public BoolChangeEvent cantSpin;
    public IntChangeEvent spinDirection;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
    }

    private void FixedUpdate()
    {
        if ((bool)cantSpin.Value == false)
            SetBodyVelocity((invertDirection ? -1f : 1f) * spinVelocity * (int)spinDirection.Value);
    }

    public void Spin(int intDirection)
    {
            spinDirection.Value = intDirection;
    }
    
    private void SetBodyVelocity(float sv)
    {
        if (spinInertia <= 0f)
            AttachedRigidbody.angularVelocity = sv;
        else
            AttachedRigidbody.angularVelocity = Mathf.MoveTowards(AttachedRigidbody.angularVelocity, sv, Time.fixedDeltaTime / spinInertia);
    }
}
