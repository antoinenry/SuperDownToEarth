using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Lever : MonoBehaviour
{
    [Min(2f)] public int numPositions = 2;
    [Min(0f)] public float angleRange = 45f;
    [Min(0f)] public float reactionDelay;
    [Min(0f)] public float rotationSpeed;

    private Rigidbody2D rb2D;
    private float leverActionTimer;

    public IntChangeEvent leverPosition;
    public IntChangeEvent leverActionDirection;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (leverPosition == null) leverPosition = new IntChangeEvent();
        leverPosition.AddValueListener<int>(OnPositionChange);
    }

    private void OnDisable()
    {
        leverPosition.RemoveValueListener<int>(OnPositionChange);
    }

    private void FixedUpdate()
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Vector2.Dot(transform.right, collision.GetContact(0).normal) < 0f)
            leverActionDirection.Value = 1;
        else
            leverActionDirection.Value = -1;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        leverActionDirection.Value = 0;
        leverActionTimer = 0f;
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
        return ((float)leverPosition / (numPositions - 1) - .5f) * angleRange + transform.parent.rotation.eulerAngles.z;
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
                transform.rotation = Quaternion.Euler(0f, 0f, wantedRotation);
        }
    }
}
