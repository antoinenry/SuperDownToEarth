using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Lever : MonoBehaviour, IValueChangeEventsComponent
{
    [Min(2f)] public int numPositions = 2;
    [Min(0f)] public float angleRange = 45f;
    [Min(0f)] public float reactionDelay;
    [Min(0f)] public float rotationSpeed;

    private Rigidbody2D rb2D;
    private float leverActionTimer;

    public ValueChangeEvent LeverPosition = ValueChangeEvent.New<int>();
    public ValueChangeEvent LeverActionDirection = ValueChangeEvent.New<int>();
    
    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { LeverPosition, LeverActionDirection };
        return vces.Length;
    }

    public int SetValueChangeEventsID()
    {
        LeverPosition.SetID("LeverPosition", this, 0);
        LeverActionDirection.SetID("LeverActionDirection", this, 1);
        return 2;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        LeverPosition.Enslave(enslave);
        LeverActionDirection.Enslave(enslave);
    }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        int leverAction = LeverActionDirection.GetValue<int>();

        if ((leverAction > 0 && LeverPosition.GetValue<int>() < numPositions - 1) || (leverAction < 0 && LeverPosition.GetValue<int>() > 0))
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
            LeverActionDirection.SetValue(1);
        else
            LeverActionDirection.SetValue(-1);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        LeverActionDirection.SetValue(0);
        leverActionTimer = 0f;
    }

    private IEnumerator RotateLeverCoroutine(int direction)
    {
        LeverPosition.SetValue(LeverPosition.GetValue<int>() + direction);

        float wantedRotation = ((float)LeverPosition.GetValue<int>() / (numPositions - 1) - .5f) * angleRange + transform.parent.rotation.eulerAngles.z;

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
