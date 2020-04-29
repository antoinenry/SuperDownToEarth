using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Lever : MonoBehaviour, IEventsHubElement
{
    public int leverPosition;
    [Min(2f)] public int numPositions = 2;
    [Min(0f)] public float angleRange = 45f;
    [Min(0f)] public float reactionDelay;
    [Min(0f)] public float rotationSpeed;

    private Rigidbody2D rb2D;
    private float leverActionTimer;

    public enum ExposedEvents { LeverActionDirection, OnLeverMove }
    public ValueChangeEvent<int> LeverActionDirection = new ValueChangeEvent<int>();
    public TriggerEvent OnLeverMove = new TriggerEvent();

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch(index)
        {
            case (int)ExposedEvents.LeverActionDirection: iValueChangeEvent = LeverActionDirection; return true;
            case (int)ExposedEvents.OnLeverMove: iValueChangeEvent = OnLeverMove; return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { typeof(int), typeof(int) };
    }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if ((LeverActionDirection.Value > 0 && leverPosition < numPositions - 1) || (LeverActionDirection.Value < 0 && leverPosition > 0))
        {
            leverActionTimer += Time.fixedDeltaTime;
            if (leverActionTimer > reactionDelay)
            {
                StartCoroutine(RotateLeverCoroutine(LeverActionDirection.Value));
                leverActionTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LeverActionDirection.Value = Vector2.Dot(transform.right, collision.GetContact(0).normal) < 0f ? 1 : -1;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        LeverActionDirection.Value = 0;
        leverActionTimer = 0f;
    }

    private IEnumerator RotateLeverCoroutine(int direction)
    {
        OnLeverMove.Trigger();
        leverPosition += direction;
        float wantedRotation = ((float)leverPosition / (numPositions - 1) - .5f) * angleRange + transform.parent.rotation.eulerAngles.z;

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
        OnLeverMove.triggered = false;
    }
}
