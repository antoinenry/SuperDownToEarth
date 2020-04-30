using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class EventsAnimator : MonoBehaviour, IValueChangeEventsComponent
{
    [Flags] public enum ParameterType
    {
        None = 0,
        Trigger = 1,
        Boolean = 2,
        Integer = 4,
        Float = 8,
        Any = 15
    }

    #region Private Events, parameter names and animator actions
    private ValueChangeEvent[] TriggerParameters = new ValueChangeEvent[0];
    private ValueChangeEvent[] BooleanParameters = new ValueChangeEvent[0];
    private ValueChangeEvent[] IntegerParameters = new ValueChangeEvent[0];
    private ValueChangeEvent[] FloatParameters = new ValueChangeEvent[0];

    private string[] triggerParameterNames = new string[0];
    private string[] booleanParameterNames = new string[0];
    private string[] integerParameterNames = new string[0];
    private string[] floatParameterNames = new string[0];

    private UnityAction[] SetTriggerActions = new UnityAction[0];
    private UnityAction<bool>[] SetBoolActions = new UnityAction<bool>[0];
    private UnityAction<int>[] SetIntActions = new UnityAction<int>[0];
    private UnityAction<float>[] SetFloatAction = new UnityAction<float>[0];
    #endregion

    #region Public counters
    public int TriggerCount { get => triggerParameterNames == null ? 0 : triggerParameterNames.Length; }
    public int BooleanCount { get => booleanParameterNames == null ? 0 : booleanParameterNames.Length; }
    public int IntegerCount { get => integerParameterNames == null ? 0 : integerParameterNames.Length; }
    public int FloatCount { get => floatParameterNames == null ? 0 : floatParameterNames.Length; }
    public int TotalEventsCount { get => TriggerCount + BooleanCount + IntegerCount + FloatCount; }
    #endregion


    #region IValueChangeEventComponent methods
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
        FetchAnimatorParameterNames(out ParameterType change);

        if (change != ParameterType.None)
        {
            UpdateEvents(change, out ParameterType updated);
            if (updated != ParameterType.None)
                SetValueChangeEventsID(updated);
        }
    }
    #endregion

    #region Set events IDs to match animator's parameter names
    private void FetchAnimatorParameterNames(out ParameterType change)
    {
        Animator animator = GetComponent<Animator>();
        List<string> triggerNames = new List<string>();
        List<string> booleanNames = new List<string>();
        List<string> integerNames = new List<string>();
        List<string> floatNames = new List<string>();

        if (animator != null)
        {
            AnimatorControllerParameter[] animatorParameters = animator.parameters;
            if (animatorParameters != null)
            {
                foreach (AnimatorControllerParameter p in animatorParameters)
                {
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
                }
            }
        }

        change = ParameterType.None;
        if (triggerParameterNames != triggerNames.ToArray())
        {
            triggerParameterNames = triggerNames.ToArray();
            change += (int)ParameterType.Trigger;
        }
        if (booleanParameterNames != booleanNames.ToArray())
        {
            booleanParameterNames = booleanNames.ToArray();
            change += (int)ParameterType.Boolean;
        }
        if (integerParameterNames != integerNames.ToArray())
        {
            integerParameterNames = integerNames.ToArray();
            change += (int)ParameterType.Integer;
        }
        if (floatParameterNames != floatNames.ToArray())
        {
            floatParameterNames = floatNames.ToArray();
            change += (int)ParameterType.Float;
        }
    }

    private void SetValueChangeEventsID(ParameterType eventTypes)
    {
        if ((eventTypes & ParameterType.Trigger) == ParameterType.Trigger) SetTriggerEventsID();
        if ((eventTypes & ParameterType.Boolean) == ParameterType.Boolean) SetBoolEventsID();
        if ((eventTypes & ParameterType.Integer) == ParameterType.Integer) SetIntEventsID();
        if ((eventTypes & ParameterType.Float) == ParameterType.Float) SetFloatEventsID();
    }

    private void SetTriggerEventsID()
    {
        if (TriggerParameters != null && triggerParameterNames != null)
            for (int i = 0; i < TriggerCount; i++)
                TriggerParameters[i].SetID(triggerParameterNames[i], this, i);
    }

    private void SetBoolEventsID()
    {
        if (BooleanParameters != null && booleanParameterNames != null)
            for (int i = 0; i < BooleanCount; i++)
                BooleanParameters[i].SetID(booleanParameterNames[i], this, i + TriggerCount);
    }

    private void SetIntEventsID()
    {
        if (IntegerParameters != null && integerParameterNames != null)
            for (int i = 0; i < IntegerCount; i++)
                IntegerParameters[i].SetID(integerParameterNames[i], this, i + TriggerCount + BooleanCount);
    }

    private void SetFloatEventsID()
    {
        if (FloatParameters != null && floatParameterNames != null)
            for (int i = 0; i < FloatCount; i++)
                FloatParameters[i].SetID(floatParameterNames[i], this, i + TriggerCount + BooleanCount + IntegerCount);
    }
    #endregion

    #region Update ValueChangeEvents arrays to match animator's parameters
    private void UpdateEvents(ParameterType updateTypes, out ParameterType change)
    {
        change = ParameterType.None;
        bool triggerChange = false, boolChange = false, intChange = false, floatChange = false;

        if ((updateTypes & ParameterType.Trigger) == ParameterType.Trigger) UpdateTriggerEvents(out triggerChange);
        if ((updateTypes & ParameterType.Boolean) == ParameterType.Boolean) UpdateBooleanEvents(out boolChange);
        if ((updateTypes & ParameterType.Integer) == ParameterType.Integer) UpdateIntegerEvents(out intChange);
        if ((updateTypes & ParameterType.Float) == ParameterType.Float) UpdateFloatEvents(out floatChange);

        if (triggerChange) change += (int)ParameterType.Trigger;
        if (boolChange) change += (int)ParameterType.Boolean;
        if (intChange) change += (int)ParameterType.Integer;
        if (floatChange) change += (int)ParameterType.Float;
    }

    private void UpdateTriggerEvents(out bool change)
    {
        change = TriggerParameters.Length != TriggerCount;

        List<ValueChangeEvent> current = new List<ValueChangeEvent>(TriggerParameters);
        ValueChangeEvent[] updated = new ValueChangeEvent[TriggerCount];
        
        for (int i = 0; i < TriggerCount; i++)
        {
            bool nochange = false;
            ValueChangeEvent match = current.Find(vce => vce.Name == triggerParameterNames[i]);            

            if (match == null)
                updated[i] = ValueChangeEvent.NewTriggerEvent();
            else if (updated[i] != match)
                updated[i] = match;
            else
                nochange = true;

            if (nochange == false) change = true;
        }

        if (change) TriggerParameters = updated;
    }

    private void UpdateBooleanEvents(out bool change)
    {
        change = BooleanParameters.Length != BooleanCount;

        List<ValueChangeEvent> current = new List<ValueChangeEvent>(BooleanParameters);
        ValueChangeEvent[] updated = new ValueChangeEvent[BooleanCount];

        for (int i = 0; i < BooleanCount; i++)
        {
            bool nochange = false;
            ValueChangeEvent match = current.Find(vce => vce.Name == booleanParameterNames[i]);

            if (match == null)
                updated[i] = ValueChangeEvent.NewValueChangeEvent<bool>();
            else if (updated[i] != match)
                updated[i] = match;
            else
                nochange = true;

            if (nochange == false) change = true;
        }

        if (change) BooleanParameters = updated;
    }

    private void UpdateIntegerEvents(out bool change)
    {
        change = IntegerParameters.Length != IntegerCount;

        List<ValueChangeEvent> current = new List<ValueChangeEvent>(IntegerParameters);
        ValueChangeEvent[] updated = new ValueChangeEvent[IntegerCount];

        for (int i = 0; i < IntegerCount; i++)
        {
            bool nochange = false;
            ValueChangeEvent match = current.Find(vce => vce.Name == integerParameterNames[i]);

            if (match == null)
                updated[i] = ValueChangeEvent.NewValueChangeEvent<int>();
            else if (updated[i] != match)
                updated[i] = match;
            else
                nochange = true;

            if (nochange == false) change = true;
        }

        if (change) IntegerParameters = updated;
    }

    private void UpdateFloatEvents(out bool change)
    {
        change = FloatParameters.Length != FloatCount;

        List<ValueChangeEvent> current = new List<ValueChangeEvent>(FloatParameters);
        ValueChangeEvent[] updated = new ValueChangeEvent[FloatCount];

        for (int i = 0; i < FloatCount; i++)
        {
            bool nochange = false;
            ValueChangeEvent match = current.Find(vce => vce.Name == floatParameterNames[i]);

            if (match == null)
                updated[i] = ValueChangeEvent.NewValueChangeEvent<float>();
            else if (updated[i] != match)
                updated[i] = match;
            else
                nochange = true;

            if (nochange == false) change = true;
        }

        if (change) FloatParameters = updated;
    }
    #endregion
    

    #region In-game Behaviour
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
    #endregion
}
