using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Jumper : BodyPart, IEventsHubElement
{
    public AnimationCurve jumpCurve;
    public bool airJump;
    public float startVelocityDamping;
    public bool showGizmo;

    public string jumpTriggerName = "jump";

    public enum ExposedEvents { onJump }
    public TriggerEvent OnJump = new TriggerEvent();
    
    public Feet Feet { get; private set; }

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
        OnJump.triggered = false;
    }

    public void Jump()
    {
        if (OnJump.triggered == false && (airJump || Feet.IsOnGround.Value == true) && Feet.IsTumbling.Value == false)
        {
            OnJump.Trigger();
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

            if (Feet.IsTumbling.Value == true) break;

            if (onGround)
            {
                if (Feet.IsOnGround.Value == false) onGround = false;
            }
            else
            {
                if (Feet.IsOnGround.Value == true) break;
            }
        }
        
        OnJump.triggered = false;
    }

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch (index)
        {
            case (int)ExposedEvents.onJump:
                iValueChangeEvent = OnJump;
                return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { null };
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }
}
