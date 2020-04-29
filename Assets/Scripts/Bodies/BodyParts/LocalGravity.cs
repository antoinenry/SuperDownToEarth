using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class LocalGravity : BodyPart, IEventsHubElement
{
    public float gravityForce;
    public float angleOffset;

    public enum ExposedEvents { isFalling }
    public ValueChangeEvent<bool> IsFalling = new ValueChangeEvent<bool>();
    
    public float FallSpeed { get; private set; }

    private UnityAction<bool> fallAnimation;   

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        //IsFalling = new ValueChangeEvent<bool>();
    }

    private void FixedUpdate ()
    {
        Vector2 localDown = Quaternion.Euler(0f, 0f, angleOffset) * transform.rotation * Vector2.down;
        AttachedRigidbody.AddForce(localDown * gravityForce);

        FallSpeed = Vector2.Dot(AttachedRigidbody.velocity, localDown);
        IsFalling.Value = FallSpeed > 0f;
    }

    public bool GetValueChangeEvent(int index, out IValueChangeEvent iValueChangeEvent)
    {
        switch (index)
        {
            case (int)ExposedEvents.isFalling:
                iValueChangeEvent = IsFalling;
                return true;
        }

        iValueChangeEvent = null;
        return false;
    }

    public void GetValueChangeEventsNamesAndTypes(out string[] names, out Type[] types)
    {
        names = Enum.GetNames(typeof(ExposedEvents));
        types = new Type[] { typeof(bool) };
    }

    public int GetValueChangeEventIndex(string vceName)
    {
        List<string> names = new List<string>(Enum.GetNames(typeof(ExposedEvents)));
        return names.IndexOf(vceName);
    }
}