using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Lever : MonoBehaviour
{
    public SimpleCollider2D handle;
    [Min(2f)] public int numPositions = 2;
    [Min(0f)] public float angleRange = 45f;
    [Min(0f)] public float reactionDelay;
    [Min(0f)] public float velocityThreshold = 0f;
    [Min(0f)] public float rotationSpeed;

    private Rigidbody2D rb2D;
    private float leverActionTimer;
    private bool enoughVelociy;

    public IntChangeEvent leverPosition;
    public IntChangeEvent leverActionDirection;

    private void Awake()
    {
        if (handle != null)
            rb2D = handle.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (leverPosition == null) leverPosition = new IntChangeEvent();
        leverPosition.AddValueListener<int>(OnPositionChange);

        if (handle != null)
        {
            handle.onCollisionEnter.AddTriggerListener(OnHandleCollisionStart);
            handle.onCollisionExit.AddTriggerListener(OnHandleCollisionEnd);
        }
    }

    private void OnDisable()
    {
        leverPosition.RemoveValueListener<int>(OnPositionChange);

        if (handle != null)
        {
            handle.onCollisionEnter.RemoveTriggerListener(OnHandleCollisionStart);
            handle.onCollisionExit.RemoveTriggerListener(OnHandleCollisionEnd);
        }
    }

    private void FixedUpdate()
    {
        if (enoughVelociy)
        {
            if ((leverActionDirection > 0 && leverPosition < numPositions - 1) || (leverActionDirection < 0 && leverPosition > 0))
            {
                leverActionTimer += Time.fixedDeltaTime;
                if (leverActionTimer > reactionDelay)
                {
                    //StartCoroutine(RotateLeverCoroutine(leverActionDirection));
                    int newLeverPosition = leverPosition + leverActionDirection;
                    leverPosition.Value = newLeverPosition;
                    float wantedRotation = GetLeverRotation(newLeverPosition);
                    StartCoroutine(RotateLeverCoroutine(wantedRotation));
                    leverActionTimer = 0f;
                }
            }
        }
    }

    private void OnHandleCollisionStart()
    {
        if (velocityThreshold > 0f)
            enoughVelociy = handle.CollisionInfos.relativeVelocity.magnitude >= velocityThreshold;
        else
            enoughVelociy = true;

        if (Vector2.Dot(handle.transform.right, handle.CollisionInfos.GetContact(0).normal) < 0f)
            leverActionDirection.Value = 1;
        else
            leverActionDirection.Value = -1;
    }

    private void OnHandleCollisionEnd()
    {
        leverActionDirection.Value = 0;
        leverActionTimer = 0f;
        enoughVelociy = false;
    }

    private IEnumerator RotateLeverCoroutine(float wantedRotation)
    {
        if (rb2D.rotation < wantedRotation)
        {
            while (rb2D.rotation < wantedRotation)
            {
                rb2D.angularVelocity = rotationSpeed;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (rb2D.rotation > wantedRotation)
            {
                rb2D.angularVelocity = -rotationSpeed;
                yield return new WaitForFixedUpdate();
            }
        }

        rb2D.MoveRotation(wantedRotation);
        rb2D.angularVelocity = 0f;
    }

    private float GetLeverRotation(int leverPosition)
    {
        return ((float)leverPosition / (numPositions - 1) - .5f) * angleRange + handle.transform.parent.rotation.eulerAngles.z;
    }

    private void OnPositionChange(int newPosition)
    {
        if (newPosition < 0 || newPosition >= numPositions)
        {
            leverPosition.Value = Mathf.Clamp(newPosition, 0, numPositions - 1);
        }
        else
        {
            float wantedRotation = GetLeverRotation(newPosition);

            if (Application.isPlaying)
                StartCoroutine(RotateLeverCoroutine(wantedRotation));
            else
                handle.transform.rotation = Quaternion.Euler(0f, 0f, wantedRotation);
        }
    }
}
