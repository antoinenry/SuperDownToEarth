using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class TestVECBehaviour : MonoBehaviour, IValueChangeEventsComponent
{
    public ValueChangeEvent triggerEvent = ValueChangeEvent.NewTriggerEvent();
    public ValueChangeEvent boolEvent = ValueChangeEvent.NewValueChangeEvent<bool>();
    public ValueChangeEvent vector2Event = ValueChangeEvent.NewValueChangeEvent<Vector2>();
    public ValueChangeEvent goEvent = ValueChangeEvent.NewValueChangeEvent<GameObject>();

    private Vector2 initPosition;

    public void SetValueChangeEventsID()
    {
        triggerEvent.SetID("triggerEvent", this, 0);
        boolEvent.SetID("boolEvent", this, 1);
        vector2Event.SetID("vector2Event", this, 2);
        goEvent.SetID("gameObjectEvent", this, 3);
    }

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { triggerEvent, boolEvent, vector2Event, goEvent };
        return vces.Length;
    }
    
    public void Awake()
    {
        initPosition = transform.position;
        vector2Event.SetValue(initPosition);
    }

    public void Start()
    {
        goEvent.AddListener<GameObject>(go => Debug.Log("GameObjectEvent: " + go));
    }

    private void Update()
    {
        if (boolEvent.GetValue<bool>() == true)
            vector2Event.SetValue(vector2Event.GetValue<Vector2>() + Vector2.up * Time.deltaTime);

        if((triggerEvent.runtimeEvent as TriggerEvent).triggered)
        {
            vector2Event.SetValue(initPosition);
            (triggerEvent.runtimeEvent as TriggerEvent).triggered = false;
        }

        transform.position = vector2Event.GetValue<Vector2>();
    }
}
