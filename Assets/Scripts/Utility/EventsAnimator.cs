using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
[RequireComponent(typeof(Animator))]
public class EventsAnimator : MonoBehaviour, IValueChangeEventsComponent
{    
    [SerializeField] private ValueChangeEvent[] TriggerParameters = new ValueChangeEvent[0];
    [SerializeField] private ValueChangeEvent[] BooleanParameters = new ValueChangeEvent[0];
    [SerializeField] private ValueChangeEvent[] IntegerParameters = new ValueChangeEvent[0];
    [SerializeField] private ValueChangeEvent[] FloatParameters = new ValueChangeEvent[0];

    [SerializeField] private string[] triggerParameterNames = new string[0];
    [SerializeField] private string[] booleanParameterNames = new string[0];
    [SerializeField] private string[] integerParameterNames = new string[0];
    [SerializeField] private string[] floatParameterNames = new string[0];

    private UnityAction[] SetTriggerActions = new UnityAction[0];
    private UnityAction<bool>[] SetBoolActions = new UnityAction<bool>[0];
    private UnityAction<int>[] SetIntActions = new UnityAction<int>[0];
    private UnityAction<float>[] SetFloatActions = new UnityAction<float>[0];
    
    public int TriggerCount { get => triggerParameterNames == null ? 0 : triggerParameterNames.Length; }
    public int BooleanCount { get => booleanParameterNames == null ? 0 : booleanParameterNames.Length; }
    public int IntegerCount { get => integerParameterNames == null ? 0 : integerParameterNames.Length; }
    public int FloatCount { get => floatParameterNames == null ? 0 : floatParameterNames.Length; }
    public int TotalEventsCount { get => TriggerCount + BooleanCount + IntegerCount + FloatCount; }

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[TotalEventsCount];
        TriggerParameters.CopyTo(vces, 0);
        BooleanParameters.CopyTo(vces, TriggerCount);
        IntegerParameters.CopyTo(vces, TriggerCount + BooleanCount);
        FloatParameters.CopyTo(vces, TriggerCount + BooleanCount + IntegerCount);

        return TotalEventsCount;
    }

    public void SetValueChangeEventsID()
    {
        FetchAnimatorParameterNames();
        UpdateEvents();

        SetValueChangeEventsID(ref TriggerParameters, triggerParameterNames, 0);
        SetValueChangeEventsID(ref BooleanParameters, booleanParameterNames, TriggerCount);
        SetValueChangeEventsID(ref IntegerParameters, integerParameterNames, TriggerCount + BooleanCount);
        SetValueChangeEventsID(ref FloatParameters, floatParameterNames, TriggerCount + BooleanCount + IntegerCount);
    }

    private void Awake()
    {
        SetValueChangeEventsID();
    }

    private void Start()
    {
        UpdateAll();
    }

    private void OnEnable()
    {
        UpdateAll();
        AddListeners(ref TriggerParameters, SetTriggerActions);
        AddListeners(ref BooleanParameters, SetBoolActions);
        AddListeners(ref IntegerParameters, SetIntActions);
        AddListeners(ref FloatParameters, SetFloatActions);
    }
    /*
    private void LateUpdate()
    {
        foreach (ValueChangeEvent vce in TriggerParameters)
            vce.Triggered = false;
    }
    */
    private void OnDisable()
    {
        RemoveListeners(ref TriggerParameters, SetTriggerActions);
        RemoveListeners(ref BooleanParameters, SetBoolActions);
        RemoveListeners(ref IntegerParameters, SetIntActions);
        RemoveListeners(ref FloatParameters, SetFloatActions);
    }

    private void AddListeners(ref ValueChangeEvent[] vces, UnityAction[] actions)
    {
        for (int i = 0, imax = vces.Length; i < imax; i++)
            vces[i].AddListener(actions[i]);
    }

    private void AddListeners<T>(ref ValueChangeEvent[] vces, UnityAction<T>[] actions)
    {
        for (int i = 0, imax = vces.Length; i < imax; i++)
            vces[i].AddListener(actions[i]);
    }

    private void RemoveListeners(ref ValueChangeEvent[] vces, UnityAction[] actions)
    {
        for (int i = 0, imax = vces.Length; i < imax; i++)
            vces[i].RemoveListener(actions[i]);
    }

    private void RemoveListeners<T>(ref ValueChangeEvent[] vces, UnityAction<T>[] actions)
    {
        for (int i = 0, imax = vces.Length; i < imax; i++)
            vces[i].RemoveListener(actions[i]);
    }

    private void FetchAnimatorParameterNames()
    {
        Animator animator = GetComponent<Animator>();
        List<string> triggerNames = new List<string>();
        List<string> booleanNames = new List<string>();
        List<string> integerNames = new List<string>();
        List<string> floatNames = new List<string>();

        AnimatorControllerParameter[] animatorParameters = animator.parameters;
        if (animatorParameters != null)
            foreach (AnimatorControllerParameter p in animatorParameters)
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Trigger:
                        triggerNames.Add(p.name); break;
                    case AnimatorControllerParameterType.Bool:
                        booleanNames.Add(p.name); break;
                    case AnimatorControllerParameterType.Int:
                        integerNames.Add(p.name); break;
                    case AnimatorControllerParameterType.Float:
                        floatNames.Add(p.name); break;
                }

        triggerParameterNames = triggerNames.ToArray();
        booleanParameterNames = booleanNames.ToArray();
        integerParameterNames = integerNames.ToArray();
        floatParameterNames = floatNames.ToArray();
    }    

    private void SetValueChangeEventsID(ref ValueChangeEvent[] vceArray, string[] namesArray, int indexInComponentOffset)
    {
        if (vceArray != null && namesArray != null)
            for (int i = 0, imax = namesArray.Length; i < imax; i++)
                vceArray[i].SetID(namesArray[i], this, i + indexInComponentOffset);
    }

    private void UpdateEvents()
    {
        UpdateValueChangeEvents<trigger>(ref TriggerParameters, triggerParameterNames);
        UpdateValueChangeEvents<bool>(ref BooleanParameters, booleanParameterNames);
        UpdateValueChangeEvents<int>(ref IntegerParameters, integerParameterNames);
        UpdateValueChangeEvents<float>(ref FloatParameters, floatParameterNames);
    }

    private void UpdateValueChangeEvents<T>(ref ValueChangeEvent[] vceArray, string[] parameterNames)
    {
        int parameterCount = parameterNames.Length;

        List<ValueChangeEvent> current = new List<ValueChangeEvent>(vceArray);
        ValueChangeEvent[] updated = new ValueChangeEvent[parameterCount];

        for (int i = 0; i < parameterCount; i++)
        {
            ValueChangeEvent match = current.Find(vce => vce.Name == parameterNames[i]);

            if (match == null)
                updated[i] = ValueChangeEvent.NewValueChangeEvent<T>();
            else if (updated[i] != match)
                updated[i] = match;
        }

        vceArray = updated;
    }

    private void UpdateActions()
    {
        Debug.Log("UpdateActions");
        Animator animator = GetComponent<Animator>();

        SetTriggerActions = new UnityAction[TriggerCount];
        for (int i = 0; i < TriggerCount; i++)
        {
            int index = i;
            SetTriggerActions[i] = new UnityAction(() =>
            {
                animator.SetTrigger(triggerParameterNames[index]);
                TriggerParameters[index].invoked = false;
            });
        }

        SetBoolActions = new UnityAction<bool>[BooleanCount];
        for (int i = 0; i < BooleanCount; i++)
        {
            int index = i;
            SetBoolActions[i] = new UnityAction<bool>(value =>
            {
                animator.SetBool(booleanParameterNames[index], value);
                BooleanParameters[index].invoked = false;
            });
        }

        SetIntActions = new UnityAction<int>[IntegerCount];
        for (int i = 0; i < IntegerCount; i++)
        {
            int index = i;
            SetIntActions[i] = new UnityAction<int>(value =>
            {
                animator.SetInteger(integerParameterNames[index], value);
                IntegerParameters[index].invoked = false;
            });
        }

        SetFloatActions = new UnityAction<float>[FloatCount];
        for (int i = 0; i < FloatCount; i++)
        {
            int index = i;
            SetFloatActions[i] = new UnityAction<float>(value =>
            {
                animator.SetFloat(floatParameterNames[index], value);
                FloatParameters[index].invoked = false;
            });
        }
    }

    private void UpdateAll()
    {
        FetchAnimatorParameterNames();
        UpdateEvents();
        UpdateActions();
    }
}
