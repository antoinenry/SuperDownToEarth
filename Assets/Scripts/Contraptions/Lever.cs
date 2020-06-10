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

    public IntChangeEvent LeverPosition;
    public IntChangeEvent LeverActionDirection;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        int leverAction = (int)LeverActionDirection.Value;

        if ((leverAction > 0 && (int)LeverPosition.Value < numPositions - 1) || (leverAction < 0 && (int)LeverPosition.Value > 0))
        {
            leverActionTimer += Time.fixedDeltaTime;
            if (leverActionTimer > reactionDelay)
            {
                StartCoroutine(RotateLeverCoroutine(leverAction));
                leverActionTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Vector2.Dot(transform.right, collision.GetContact(0).normal) < 0f)
            LeverActionDirection.Value = 1;
        else
            LeverActionDirection.Value = -1;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        LeverActionDirection.Value = 0;
        leverActionTimer = 0f;
    }

    private IEnumerator RotateLeverCoroutine(int direction)
    {
        int newLeverPosition = LeverPosition + direction;
        LeverPosition.Value = newLeverPosition;

        float wantedRotation = ((float)newLeverPosition / (numPositions - 1) - .5f) * angleRange + transform.parent.rotation.eulerAngles.z;

        if (direction > 0)
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
}
