using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class LocalGravity : BodyPart
{
    public float gravityForce;
    public float angleOffset;

    public ValueChangeEvent IsFalling = ValueChangeEvent.New<bool>();

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
        IsFalling.Set(FallSpeed > 0f);
    }
}