﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Feet : BodyPart, IValueChangeEventsComponent
{
    [Range(0f,180f)] public float balanceAngle = 180f;
    public AnimationCurve tumbleBounceCurve;
    public float tumbleSpinVelocity = 360f;
    public string cantStandOnTag = "Unwalkable";
    public float cornerAngleAdjustmentSpeed = 180f;

    [Header("Events")]
    public ValueChangeEvent IsOnGround = ValueChangeEvent.New<bool>();
    public ValueChangeEvent IsTumbling = ValueChangeEvent.New<bool>();

    private FlatGroundProbe groundProbe;
    //private Joint2D groundJoint;
    private Rigidbody2D groundRigidbody;

    private List<ContactPoint2D> groundContacts;
    private int groundCount;

    public float groundAngle { get; private set; }
    public Vector2 GroundVelocity { get => groundRigidbody == null ? Vector2.zero : groundRigidbody.GetPointVelocity(this.transform.position); }
    public float GroundAngularVelocity { get => groundRigidbody == null ? 0f : groundRigidbody.angularVelocity; }
    
    public int SetValueChangeEventsID()
    {
        IsOnGround.SetID("IsOnGround", this, 0);
        IsTumbling.SetID("IsTumbling", this, 1);
        return 2;
    }

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { IsOnGround, IsTumbling };
        return vces.Length;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        IsOnGround.Enslave(enslave);
        IsTumbling.Enslave(enslave);
    }

    void Awake()
    {
        //_IsOnGround = new ValueChangeEvent<bool>();
        //_IsTumbling = new ValueChangeEvent<bool>();

        AttachedBody = GetComponent<Body>();
        groundProbe = GetComponentInChildren<FlatGroundProbe>();
        //groundJoint = GetComponent<Joint2D>();

        groundContacts = new List<ContactPoint2D>();
        groundCount = 0;
    }

    private void FixedUpdate()
    {
        CheckGround();
        groundContacts.Clear();

        if (groundCount > 0 && IsTumbling.Get<bool>() == false)
        {
            AttachedRigidbody.velocity = GroundVelocity;
            AttachedRigidbody.angularVelocity = GroundAngularVelocity;
            AttachedRigidbody.rotation = groundAngle;            

            if (IsOnGround.Get<bool>() == true)
            {
                //if (groundProbe != null) AdjustRotationOnCorner();
            }
            else
                IsOnGround.Set(true);
        }
        else if (groundProbe == null || groundProbe.GroundFlatness.Get<int>() == (int)FlatGroundProbe.Flatness.NoGround)
        {
            AttachedRigidbody.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
            IsOnGround.Set(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(cantStandOnTag))
            return;

        groundCount++;
        groundContacts.AddRange(collision.contacts);

        if (collision.rigidbody != null) groundRigidbody = collision.rigidbody;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(cantStandOnTag))
            return;

        groundCount--;

        if (collision.rigidbody == groundRigidbody) groundRigidbody = null;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(cantStandOnTag))
            return;

        groundContacts.AddRange(collision.contacts);
    }
    
    private void OnDisable()
    {
        groundRigidbody = null;
    }
    
    private void CheckGround()
    {
        if (groundContacts.Count == 0) return;

        Vector2 tumbleDirection = Vector2.zero;
        float angleDiff = float.PositiveInfinity;
        bool balanced = false;

        foreach (ContactPoint2D contact in groundContacts)
        {
            float thisAngleDiff = Vector2.Angle(contact.normal, transform.rotation * Vector2.up);
            float thisGroundAngle = Vector2.SignedAngle(Vector2.up, contact.normal);

            if (thisAngleDiff <= balanceAngle && thisAngleDiff < angleDiff)
            {
                groundAngle = thisGroundAngle;
                angleDiff = thisAngleDiff;
                balanced = true;
            }

            tumbleDirection += contact.normal;
        }

        IsTumbling.Set(!balanced);
        if (balanced == false) StartCoroutine(TumbleCoroutine(tumbleDirection.normalized));
    }

    private IEnumerator TumbleCoroutine(Vector2 direction)
    {
        Vector2 startPosition = AttachedRigidbody.position;        
        float currentTime = 0f;
        float currentHeight = tumbleBounceCurve.Evaluate(0f);
        float lastHeight = currentHeight;
        float tumbleDuration = tumbleBounceCurve.keys[tumbleBounceCurve.length - 1].time;

        //bool clockwiseSpin = Vector2.SignedAngle(rb.velocity, direction) > 0f;
        AttachedRigidbody.velocity = Vector2.zero;

        while (IsTumbling.Get<bool>() ==true && currentTime < tumbleDuration)
        {
            lastHeight = currentHeight;
            currentHeight = tumbleBounceCurve.Evaluate(currentTime);

            AttachedRigidbody.MovePosition(startPosition + direction * currentHeight);
            AttachedRigidbody.angularVelocity = tumbleSpinVelocity;

            yield return new WaitForFixedUpdate();
            currentTime += Time.fixedDeltaTime;
        }

        AttachedRigidbody.velocity = direction * (currentHeight - lastHeight)/Time.fixedDeltaTime;
        IsTumbling.Set(false);
    }

    private void AdjustRotationOnCorner()
    {
        if (groundProbe.GroundFlatness.Get<int>() ==(int)FlatGroundProbe.Flatness.Hole)
        {
            float facing = groundProbe.transform.lossyScale.x > 0 ? 1f : -1f;

            if (groundProbe.FrontUpCollision == true && groundProbe.BackUpCollision == false)
                AttachedRigidbody.rotation += facing * cornerAngleAdjustmentSpeed * Time.fixedDeltaTime;
            else if (groundProbe.FrontUpCollision == false && groundProbe.BackUpCollision == true)
                AttachedRigidbody.rotation -= facing * cornerAngleAdjustmentSpeed * Time.fixedDeltaTime;
        }
        else
            AttachedRigidbody.constraints |= RigidbodyConstraints2D.FreezeRotation;
    }
}
