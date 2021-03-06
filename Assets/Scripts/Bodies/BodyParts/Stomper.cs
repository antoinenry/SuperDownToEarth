﻿using System.Collections;
using UnityEngine;

public class Stomper : BodyPart
{
    public LocalGravity gravity;
    public float velocityThreshold;

    public BoolChangeEvent velocityReached;
    public Trigger stomp;
    private Collider2D stompCollider;

    private void Awake()
    {
        stompCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        velocityReached.AddValueListener<bool>(OnVelocityReached);

        if (gravity != null)
        {
            gravity.isFalling.AddValueListener<bool>(OnFall);
            //OnFall((bool)gravity.isFalling.Value);
        }
    }

    private void OnDisable()
    {
        if (stompCollider != null)
            stompCollider.enabled = false;
        velocityReached.RemoveValueListener<bool>(OnVelocityReached);

        if (gravity != null)
            gravity.isFalling.RemoveValueListener<bool>(OnFall);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        stomp.Trigger();
    }

    private void OnVelocityReached(bool reached)
    {
        if (stompCollider != null)
            stompCollider.enabled = reached;
    }

    private void OnFall(bool isFalling)
    {
        StopAllCoroutines();
        if (isFalling)
            StartCoroutine(FallCoroutine());
        else if (velocityReached)
            velocityReached.Value = false;
    }

    private IEnumerator FallCoroutine()
    {
        while ((bool)gravity.isFalling.Value == true)
        {
            if (gravity.fallSpeed >= velocityThreshold)
                velocityReached.Value = true;

            yield return new WaitForFixedUpdate();
        }
    }
}
