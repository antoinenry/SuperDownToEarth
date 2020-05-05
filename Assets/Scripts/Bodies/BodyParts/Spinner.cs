using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : BodyPart
{
    public bool invertDirection = true;
    public float spinVelocity;
    public float spinInertia;
    public float tumbleRecuperation;
    
    public Feet Feet { get; private set; }
    
    public ValueChangeEvent spinDirection = ValueChangeEvent.New<int>();
    
    public override int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { spinDirection };
        return 1;
    }

    public override int SetValueChangeEventsID()
    {
        spinDirection.SetID("spinDirection", this, 0);
        return 1;
    }

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
    }

    private void FixedUpdate()
    {
        SetBodyVelocity((invertDirection ? -1f : 1f) * spinVelocity * spinDirection.Get<int>());
    }

    public void Spin(int intDirection)
    {
        spinDirection.Set(intDirection);
    }
    
    private void SetBodyVelocity(float sv)
    {
        if (spinInertia <= 0f)
            AttachedRigidbody.angularVelocity = sv;
        else
            AttachedRigidbody.angularVelocity = Mathf.MoveTowards(AttachedRigidbody.angularVelocity, sv, Time.fixedDeltaTime / spinInertia);
    }
}
