using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class EventsAnimator : MonoBehaviour, IPausable
{
    [SerializeField] private AnimatorTriggerEvent[] animatorTriggers;
    [SerializeField] private AnimatorBoolEvent[] animatorBools;
    [SerializeField] private AnimatorIntEvent[] animatorInts;
    [SerializeField] private AnimatorFloatEvent[] animatorFloats;

    [SerializeField] private UnityAction[] setTrigger;
    [SerializeField] private UnityAction<bool>[] setBool;
    [SerializeField] private UnityAction<int>[] setInt;
    [SerializeField] private UnityAction<float>[] setFloat;    

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        foreach (AnimatorTriggerEvent t in animatorTriggers)
            t.VCE.ListenToMasters();
        foreach (AnimatorBoolEvent b in animatorBools)
            b.VCE.ListenToMasters();
        foreach (AnimatorIntEvent i in animatorInts)
            i.VCE.ListenToMasters();
        foreach (AnimatorFloatEvent f in animatorFloats)
            f.VCE.ListenToMasters();
    }

    private void OnDisable()
    {
        foreach (AnimatorTriggerEvent t in animatorTriggers)
            t.VCE.IgnoreMasters();
        foreach (AnimatorBoolEvent b in animatorBools)
            b.VCE.IgnoreMasters();
        foreach (AnimatorIntEvent i in animatorInts)
            i.VCE.IgnoreMasters();
        foreach (AnimatorFloatEvent f in animatorFloats)
            f.VCE.IgnoreMasters();
    }

    private void OnDestroy()
    {
        for (int i = 0; i< ParameterCount; i++)
            GetParameterEvent(i).VCE.RemoveAllListeners();
    }

    public void Init()
    {
        UnsuscribeAnimatorActionsFromValueChangeEvents();
        SetAnimatorValueChangeEvents();
        SetAnimatorActions();
        SuscribeAnimatorActionsToValueChangeEvents();
    }
    
    public int TriggerCount { get => animatorTriggers == null ? 0 : animatorTriggers.Length; }
    public int BoolCount { get => animatorBools == null ? 0 : animatorBools.Length; }
    public int IntCount { get => animatorInts == null ? 0 : animatorInts.Length; }
    public int FloatCount { get => animatorFloats == null ? 0 : animatorFloats.Length; }
    public int ParameterCount { get => TriggerCount + BoolCount + IntCount + FloatCount;  }

    public AnimatorParameterEvent GetParameterEvent(int index)
    {
        if (index >= 0)
        {
            if (index < TriggerCount) return animatorTriggers[index];
            index -= TriggerCount;
            if (index < BoolCount) return animatorBools[index];
            index -= BoolCount;
            if (index < IntCount) return animatorInts[index];
            index -= IntCount;
            if (index < FloatCount) return animatorFloats[index];
        }

        return null;
    }

    private void SetAnimatorValueChangeEvents()
    {
        List<AnimatorParameterEvent> updatedParameterEvents = new List<AnimatorParameterEvent>();

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            List<AnimatorParameterEvent> currentParameterEvents = new List<AnimatorParameterEvent>();
            if (animatorTriggers != null) currentParameterEvents.AddRange(animatorTriggers);
            if (animatorBools != null) currentParameterEvents.AddRange(animatorBools);
            if (animatorInts != null) currentParameterEvents.AddRange(animatorInts);
            if (animatorFloats != null) currentParameterEvents.AddRange(animatorFloats);

            if (animator.isInitialized == false) animator.Rebind();                        
            int parameterCount = animator.parameterCount;

            for (int i = 0; i < parameterCount; i++)
            {
                AnimatorControllerParameter p = animator.GetParameter(i);
                int currentIndex = currentParameterEvents.FindIndex(x => x.VCE != null && x.name == p.name && x.Type == p.type);
                if (currentIndex >= 0) updatedParameterEvents.Add(currentParameterEvents[currentIndex]);
                else updatedParameterEvents.Add(AnimatorParameterEvent.New(p));
            }  
        }
        
        animatorTriggers = updatedParameterEvents.FindAll(p => p.Type == AnimatorControllerParameterType.Trigger).ConvertAll(t => t as AnimatorTriggerEvent).ToArray();
        animatorBools = updatedParameterEvents.FindAll(p => p.Type == AnimatorControllerParameterType.Bool).ConvertAll(t => t as AnimatorBoolEvent).ToArray();
        animatorInts = updatedParameterEvents.FindAll(p => p.Type == AnimatorControllerParameterType.Int).ConvertAll(t => t as AnimatorIntEvent).ToArray();
        animatorFloats = updatedParameterEvents.FindAll(p => p.Type == AnimatorControllerParameterType.Float).ConvertAll(t => t as AnimatorFloatEvent).ToArray();
    }

    private void SetAnimatorActions()
    {
        List<UnityAction> triggerActions = new List<UnityAction>();
        List<UnityAction<bool>> boolActions = new List<UnityAction<bool>>();
        List<UnityAction<int>> intActions = new List<UnityAction<int>>();
        List<UnityAction<float>> floatActions = new List<UnityAction<float>>();

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            if (animator.isInitialized == false) animator.Rebind();

            for (int i = 0; i < ParameterCount; i++)
            {
                AnimatorParameterEvent p = GetParameterEvent(i);
                switch (p.Type)
                {
                    case AnimatorControllerParameterType.Trigger:
                        triggerActions.Add(new UnityAction(() => animator.SetTrigger(p.name)));
                        break;
                    case AnimatorControllerParameterType.Bool:
                        boolActions.Add(new UnityAction<bool>(x => animator.SetBool(p.name, x)));
                        break;
                    case AnimatorControllerParameterType.Int:
                        intActions.Add(new UnityAction<int>(x => animator.SetInteger(p.name, x)));
                        break;
                    case AnimatorControllerParameterType.Float:
                        floatActions.Add(new UnityAction<float>(x => animator.SetFloat(p.name, x)));
                        break;
                }
            }
        }

        setTrigger = triggerActions.ToArray();
        setBool = boolActions.ToArray();
        setInt = intActions.ToArray();
        setFloat = floatActions.ToArray();
    }

    private void SuscribeAnimatorActionsToValueChangeEvents()
    {
        for (int i = 0; i < TriggerCount; i++)
            animatorTriggers[i].VCE.AddTriggerListener(setTrigger[i]);
        for (int i = 0; i < BoolCount; i++)
            animatorBools[i].VCE.AddValueListener(setBool[i]);
        for (int i = 0; i < IntCount; i++)
            animatorInts[i].VCE.AddValueListener(setInt[i]);
        for (int i = 0; i < FloatCount; i++)
            animatorFloats[i].VCE.AddValueListener(setFloat[i]);
    }

    private void UnsuscribeAnimatorActionsFromValueChangeEvents()
    {
        if (setTrigger != null)
            for (int i = 0; i < TriggerCount; i++)
                animatorTriggers[i].VCE.RemoveTriggerListener(setTrigger[i]);
        if (setBool != null)
            for (int i = 0; i < BoolCount; i++)
                animatorBools[i].VCE.RemoveValueListener(setBool[i]);
        if (setInt != null)
            for (int i = 0; i < IntCount; i++)
                animatorInts[i].VCE.RemoveValueListener(setInt[i]);
        if (setFloat != null)
            for (int i = 0; i < FloatCount; i++)
                animatorFloats[i].VCE.RemoveValueListener(setFloat[i]);
    }

    public void Pause(bool pause)
    {
        Animator animator = GetComponent<Animator>();
        animator.enabled = !pause;
    }
}
