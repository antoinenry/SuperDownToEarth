using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
[RequireComponent(typeof(Animator))]
public class EventsAnimator : MonoBehaviour, IValueChangeEventsComponent
{    
    [SerializeField] private ValueChangeEvent[] TriggerParameters = new ValueChangeEvent[0];
    [SerializeField] private ValueChangeEvent[] BootParameters = new ValueChangeEvent[0];
    [SerializeField] private ValueChangeEvent[] IntParameters = new ValueChangeEvent[0];
    [SerializeField] private ValueChangeEvent[] FloatParameters = new ValueChangeEvent[0];

    [SerializeField] private string[] triggerParameterNames = new string[0];
    [SerializeField] private string[] boolParameterNames = new string[0];
    [SerializeField] private string[] intParameterNames = new string[0];
    [SerializeField] private string[] floatParameterNames = new string[0];

    private UnityAction[] SetTriggerActions = new UnityAction[0];
    private UnityAction<bool>[] SetBoolActions = new UnityAction<bool>[0];
    private UnityAction<int>[] SetIntActions = new UnityAction<int>[0];
    private UnityAction<float>[] SetFloatActions = new UnityAction<float>[0];
    
    public int TriggerCount { get => triggerParameterNames == null ? 0 : triggerParameterNames.Length; }
    public int BooleanCount { get => boolParameterNames == null ? 0 : boolParameterNames.Length; }
    public int IntegerCount { get => intParameterNames == null ? 0 : intParameterNames.Length; }
    public int FloatCount { get => floatParameterNames == null ? 0 : floatParameterNames.Length; }
    public int TotalEventsCount { get => TriggerCount + BooleanCount + IntegerCount + FloatCount; }

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[TotalEventsCount];
        TriggerParameters.CopyTo(vces, 0);
        BootParameters.CopyTo(vces, TriggerCount);
        IntParameters.CopyTo(vces, TriggerCount + BooleanCount);
        FloatParameters.CopyTo(vces, TriggerCount + BooleanCount + IntegerCount);

        return TotalEventsCount;
    }

    public int SetValueChangeEventsID()
    {
        FetchAnimatorParameterNames();
        UpdateEvents();

        SetValueChangeEventsID(ref TriggerParameters, triggerParameterNames, 0);
        SetValueChangeEventsID(ref BootParameters, boolParameterNames, TriggerCount);
        SetValueChangeEventsID(ref IntParameters, intParameterNames, TriggerCount + BooleanCount);
        SetValueChangeEventsID(ref FloatParameters, floatParameterNames, TriggerCount + BooleanCount + IntegerCount);

        return TotalEventsCount;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        foreach (ValueChangeEvent vce in TriggerParameters) vce.Enslave(enslave);
        foreach (ValueChangeEvent vce in BootParameters) vce.Enslave(enslave);
        foreach (ValueChangeEvent vce in IntParameters) vce.Enslave(enslave);
        foreach (ValueChangeEvent vce in FloatParameters)   vce.Enslave(enslave);
    }

    private void Awake()
    {
        SetValueChangeEventsID();
    }

    private void OnEnable()
    {
        UpdateEvents();
        UpdateActions();
        EnslaveValueChangeEvents(true);
        AddAnimatorListeners();
    }

    private void OnDisable()
    {
        EnslaveValueChangeEvents(false);
        RemoveAnimatorListeners();
    }      

    private void AddAnimatorListeners()
    {
        for (int i = 0, imax = TriggerParameters.Length; i < imax; i++) TriggerParameters[i].AddListener(SetTriggerActions[i]);
        for (int i = 0, imax = BootParameters.Length; i < imax; i++) BootParameters[i].AddListener(SetBoolActions[i]);
        for (int i = 0, imax = IntParameters.Length; i < imax; i++) IntParameters[i].AddListener(SetIntActions[i]);
        for (int i = 0, imax = FloatParameters.Length; i < imax; i++) FloatParameters[i].AddListener(SetFloatActions[i]);
    }

    private void RemoveAnimatorListeners()
    {
        for (int i = 0, imax = TriggerParameters.Length; i < imax; i++) TriggerParameters[i].RemoveListener(SetTriggerActions[i]);
        for (int i = 0, imax = BootParameters.Length; i < imax; i++) BootParameters[i].RemoveListener(SetBoolActions[i]);
        for (int i = 0, imax = IntParameters.Length; i < imax; i++) IntParameters[i].RemoveListener(SetIntActions[i]);
        for (int i = 0, imax = FloatParameters.Length; i < imax; i++) FloatParameters[i].RemoveListener(SetFloatActions[i]);
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
        boolParameterNames = booleanNames.ToArray();
        intParameterNames = integerNames.ToArray();
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
        UpdateValueChangeEvents<bool>(ref BootParameters, boolParameterNames);
        UpdateValueChangeEvents<int>(ref IntParameters, intParameterNames);
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
                updated[i] = ValueChangeEvent.New<T>();
            else
            {
                if (match.runtimeEvent == null) match.ResetRuntimeEvent<T>();
                if (updated[i] != match) updated[i] = match;
            }
        }

        vceArray = updated;
    }

    private void UpdateActions()
    {
        Animator animator = GetComponent<Animator>();

        SetTriggerActions = new UnityAction[TriggerCount];
        for (int i = 0; i < TriggerCount; i++)
        {
            int index = i;
            SetTriggerActions[i] = new UnityAction(() => animator.SetTrigger(triggerParameterNames[index]));
        }

        SetBoolActions = new UnityAction<bool>[BooleanCount];
        for (int i = 0; i < BooleanCount; i++)
        {
            int index = i;
            SetBoolActions[i] = new UnityAction<bool>(value => animator.SetBool(boolParameterNames[index], value));
        }

        SetIntActions = new UnityAction<int>[IntegerCount];
        for (int i = 0; i < IntegerCount; i++)
        {
            int index = i;
            SetIntActions[i] = new UnityAction<int>(value => animator.SetInteger(intParameterNames[index], value));
        }

        SetFloatActions = new UnityAction<float>[FloatCount];
        for (int i = 0; i < FloatCount; i++)
        {
            int index = i;
            SetFloatActions[i] = new UnityAction<float>(value => animator.SetFloat(floatParameterNames[index], value));
        }
    }
}
