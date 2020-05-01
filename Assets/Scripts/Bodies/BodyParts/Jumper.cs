﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Jumper : BodyPart, IValueChangeEventsComponent
{
    public AnimationCurve jumpCurve;
    public bool airJump;
    public float startVelocityDamping;
    public bool showGizmo;
        
    public ValueChangeEvent OnJump = ValueChangeEvent.NewTriggerEvent();
    
    public Feet Feet { get; private set; }

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { OnJump };
        return vces.Length;
    }

    public void SetValueChangeEventsID()
    {
        OnJump.SetID("onJump", this, 0);
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        OnJump.Enslave(enslave);
    }

    private void OnDrawGizmos()
    {
        if(showGizmo)
        {
            Gizmos.color = Color.grey;

            Vector3 pt = transform.position;
            Vector3 jumpDirection = transform.rotation * Vector2.up;

            foreach (Keyframe kf in jumpCurve.keys)
            {
                Vector3 nextPt = transform.position + jumpDirection * kf.value;
                Gizmos.DrawLine(pt, nextPt);
                pt = nextPt;
            }
        }
    }

    private void Awake()
    {
        //OnJump = new TriggerEvent();

        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        OnJump.invoked = false;
    }

    public void Jump()
    {
        if (OnJump.invoked == false && (airJump || Feet.IsOnGround.GetValue<bool>() == true) && Feet.IsTumbling.GetValue<bool>() == false)
        {
            OnJump.Invoke();
            StartCoroutine(JumpCoroutine());
        }
    }

    private IEnumerator JumpCoroutine()
    {
        Vector2 jumpDirection = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.up;
        Vector2 startPosition = AttachedRigidbody.position;
        
        float currentTime = 0f;
        Vector2 currentPosition = startPosition;
        Vector2 lastPosition = startPosition;
        float jumpDuration = jumpCurve.keys[jumpCurve.length - 1].time;
        Vector2 additionnalVelocity = Feet.GroundVelocity;
        bool onGround = true;
        
        while (AttachedRigidbody.simulated == true)
        {
            float deltaTime = Time.fixedDeltaTime;
            additionnalVelocity = Vector2.MoveTowards(additionnalVelocity, Vector2.zero, startVelocityDamping * deltaTime);
            currentTime += deltaTime;

            if (currentTime >= jumpDuration)
            {
                AttachedRigidbody.velocity = (currentPosition - lastPosition) / deltaTime;
                break;
            }
            
            float currentHeight = jumpCurve.Evaluate(currentTime);
            lastPosition = currentPosition;
            currentPosition = startPosition + jumpDirection * currentHeight + additionnalVelocity * currentTime;
            AttachedRigidbody.MovePosition(currentPosition);

            yield return new WaitForFixedUpdate();

            if (Feet.IsTumbling.GetValue<bool>() == true) break;

            if (onGround)
            {
                if (Feet.IsOnGround.GetValue<bool>() == false) onGround = false;
            }
            else
            {
                if (Feet.IsOnGround.GetValue<bool>() == true) break;
            }
        }
        
        OnJump.invoked = false;
    }
}
