using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class TestVECBehaviour : MonoBehaviour, IValueChangeEventsComponent
{
    public ValueChangeEvent triggerEvent = ValueChangeEvent.New<trigger>();
    public ValueChangeEvent boolEvent = ValueChangeEvent.New<bool>();
    public ValueChangeEvent vector2Event = ValueChangeEvent.New<Vector2>();
    public ValueChangeEvent goEvent = ValueChangeEvent.New<GameObject>();

    public Vector2 initPosition;
    public Vector2 move = Vector2.up;

    public int SetValueChangeEventsID()
    {
        triggerEvent.SetID("triggerEvent", this, 0);
        boolEvent.SetID("boolEvent", this, 1);
        vector2Event.SetID("vector2Event", this, 2);
        goEvent.SetID("gameObjectEvent", this, 3);
        return 4;
    }

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { triggerEvent, boolEvent, vector2Event, goEvent };
        return vces.Length;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        triggerEvent.Enslave(enslave);
        boolEvent.Enslave(enslave);
        vector2Event.Enslave(enslave);
        goEvent.Enslave(enslave);
    }
    
    public void Awake()
    {
        initPosition = transform.position;
        vector2Event.SetValue(initPosition);
    }

    public void OnEnable()
    {
        EnslaveValueChangeEvents(true);

        triggerEvent.AddListener(OnTrigger);
        boolEvent.AddListener<bool>(OnBool);
        vector2Event.AddListener<Vector2>(OnV2);
    }

    public void OnDisable()
    {
        EnslaveValueChangeEvents(false);

        triggerEvent.RemoveListener(OnTrigger);
        boolEvent.RemoveListener<bool>(OnBool);
        vector2Event.RemoveListener<Vector2>(OnV2);
    }

    private void OnV2(Vector2 v2)
    {
        transform.position = v2;
    }

    private void OnBool(bool b)
    {
        if (b) StartCoroutine(MoveV2());
        else StopCoroutine(MoveV2());
    }

    private void OnTrigger()
    {
        vector2Event.SetValue(initPosition);
    }

    IEnumerator MoveV2()
    {
        while (boolEvent.GetValue<bool>() == true)
        {
            vector2Event.SetValue(vector2Event.GetValue<Vector2>() + move * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
}
