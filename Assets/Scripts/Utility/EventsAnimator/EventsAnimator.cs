using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class EventsAnimator : ValueChangeEventsBehaviour
{    
    [SerializeField] private ValueChangeEvent[] TriggerParameters = new ValueChangeEvent[0];
    [SerializeField] private ValueChangeEvent[] BoolParameters = new ValueChangeEvent[0];
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
    public int BoolCount { get => boolParameterNames == null ? 0 : boolParameterNames.Length; }
    public int IntCount { get => intParameterNames == null ? 0 : intParameterNames.Length; }
    public int FloatCount { get => floatParameterNames == null ? 0 : floatParameterNames.Length; }
    public int TotalEventsCount { get => TriggerCount + BoolCount + IntCount + FloatCount; }

    public override void Awake()
    {
        base.Awake();
        SetAnimatorActions();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        AddAnimatorListeners();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        RemoveAnimatorListeners();
    }

    public override int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[TotalEventsCount];
        TriggerParameters.CopyTo(vces, 0);
        BoolParameters.CopyTo(vces, TriggerCount);
        IntParameters.CopyTo(vces, TriggerCount + BoolCount);
        FloatParameters.CopyTo(vces, TriggerCount + BoolCount + IntCount);

        return TotalEventsCount;
    }

    public override ValueChangeEvent GetValueChangeEventByName(string vceName)
    {
        for (int i = 0; i < TriggerCount; i++)  if (triggerParameterNames[i] == vceName)    return i < TriggerParameters.Length ? TriggerParameters[i] : null;
        for (int i = 0; i < BoolCount; i++)     if (boolParameterNames[i] == vceName)       return i < BoolParameters.Length ? BoolParameters[i] : null;
        for (int i = 0; i < IntCount; i++)      if (intParameterNames[i] == vceName)        return i < IntParameters.Length ? IntParameters[i] : null;
        for (int i = 0; i < FloatCount; i++)    if (floatParameterNames[i] == vceName)      return i < FloatParameters.Length ? FloatParameters[i] : null;

        return null;
    }

    public override string[] GetValueChangeEventsNames()
    {
        string[] vceNames = new string[TotalEventsCount];
        triggerParameterNames.CopyTo(vceNames, 0);
        boolParameterNames.CopyTo(vceNames, TriggerCount);
        intParameterNames.CopyTo(vceNames, TriggerCount + BoolCount);
        floatParameterNames.CopyTo(vceNames, TriggerCount + BoolCount + IntCount);

        return vceNames;
    }

    public void SetValueChangeEventsFromAnimator(Animator animator)
    {
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

        UpdateValueChangeEvents<trigger>(ref TriggerParameters, triggerParameterNames);
        UpdateValueChangeEvents<bool>(ref BoolParameters, boolParameterNames);
        UpdateValueChangeEvents<int>(ref IntParameters, intParameterNames);
        UpdateValueChangeEvents<float>(ref FloatParameters, floatParameterNames);
    }

    private void AddAnimatorListeners()
    {
        for (int i = 0, imax = TriggerParameters.Length; i < imax; i++) TriggerParameters[i].AddListener(SetTriggerActions[i]);
        for (int i = 0, imax = BoolParameters.Length; i < imax; i++) BoolParameters[i].AddListener(SetBoolActions[i]);
        for (int i = 0, imax = IntParameters.Length; i < imax; i++) IntParameters[i].AddListener(SetIntActions[i]);
        for (int i = 0, imax = FloatParameters.Length; i < imax; i++) FloatParameters[i].AddListener(SetFloatActions[i]);
    }

    private void RemoveAnimatorListeners()
    {
        for (int i = 0, imax = TriggerParameters.Length; i < imax; i++) TriggerParameters[i].RemoveListener(SetTriggerActions[i]);
        for (int i = 0, imax = BoolParameters.Length; i < imax; i++) BoolParameters[i].RemoveListener(SetBoolActions[i]);
        for (int i = 0, imax = IntParameters.Length; i < imax; i++) IntParameters[i].RemoveListener(SetIntActions[i]);
        for (int i = 0, imax = FloatParameters.Length; i < imax; i++) FloatParameters[i].RemoveListener(SetFloatActions[i]);
    }

    private void UpdateValueChangeEvents<T>(ref ValueChangeEvent[] vceArray, string[] parameterNames)
    {
        int parameterCount = parameterNames.Length;

        List<ValueChangeEvent> current = new List<ValueChangeEvent>(vceArray);
        ValueChangeEvent[] updated = new ValueChangeEvent[parameterCount];

        for (int i = 0; i < parameterCount; i++)
        {            
            ValueChangeEvent match = GetValueChangeEventByName(parameterNames[i]);

            if (match == null)
                updated[i] = ValueChangeEvent.New<T>();
            else
            {
                if (match.runtimeEvent == null) match.SetRuntimeEvent<T>();
                if (updated[i] != match) updated[i] = match;
            }
        }

        vceArray = updated;
    }

    private void SetAnimatorActions()
    {
        Animator animator = GetComponent<Animator>();

        SetTriggerActions = new UnityAction[TriggerCount];
        for (int i = 0; i < TriggerCount; i++)
        {
            int index = i;
            SetTriggerActions[i] = new UnityAction(() => animator.SetTrigger(triggerParameterNames[index]));
        }

        SetBoolActions = new UnityAction<bool>[BoolCount];
        for (int i = 0; i < BoolCount; i++)
        {
            int index = i;
            SetBoolActions[i] = new UnityAction<bool>(value => animator.SetBool(boolParameterNames[index], value));
        }

        SetIntActions = new UnityAction<int>[IntCount];
        for (int i = 0; i < IntCount; i++)
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
