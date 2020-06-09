using System;
using UnityEngine;
using VCE;

public abstract class AnimatorParameterEvent
{
    public string name;

    public abstract ValueChangeEvent VCE { get; }
    public abstract AnimatorControllerParameterType Type { get; }

    public static AnimatorParameterEvent New(AnimatorControllerParameter parameter)
    {
        switch (parameter.type)
        {
            case AnimatorControllerParameterType.Trigger: return new AnimatorTriggerEvent(parameter.name);
            case AnimatorControllerParameterType.Bool: return new AnimatorBoolEvent(parameter.name);
            case AnimatorControllerParameterType.Int: return new AnimatorIntEvent(parameter.name);
            case AnimatorControllerParameterType.Float: return new AnimatorFloatEvent(parameter.name);
        }
        return null;
    }
}

[Serializable]
public class AnimatorTriggerEvent : AnimatorParameterEvent
{
    [SerializeField] private Trigger vce;

    public override ValueChangeEvent VCE { get => vce; }
    public override AnimatorControllerParameterType Type { get => AnimatorControllerParameterType.Trigger; }

    public AnimatorTriggerEvent(string parameterName)
    {
        name = parameterName;
        vce = new Trigger();
    }
}

[Serializable]
public class AnimatorBoolEvent : AnimatorParameterEvent
{
    [SerializeField] private BoolChangeEvent vce;

    public override ValueChangeEvent VCE { get => vce; }
    public override AnimatorControllerParameterType Type { get => AnimatorControllerParameterType.Bool; }

    public AnimatorBoolEvent(string parameterName)
    {
        name = parameterName;
        vce = new BoolChangeEvent();
    }
}

[Serializable]
public class AnimatorIntEvent : AnimatorParameterEvent
{
    [SerializeField] private IntChangeEvent vce;

    public override ValueChangeEvent VCE { get => vce; }
    public override AnimatorControllerParameterType Type { get => AnimatorControllerParameterType.Int; }

    public AnimatorIntEvent(string parameterName)
    {
        name = parameterName;
        vce = new IntChangeEvent();
    }
}

[Serializable]
public class AnimatorFloatEvent : AnimatorParameterEvent
{
    [SerializeField] private FloatChangeEvent vce;

    public override ValueChangeEvent VCE { get => vce; }
    public override AnimatorControllerParameterType Type { get => AnimatorControllerParameterType.Float; }

    public AnimatorFloatEvent(string parameterName)
    {
        name = parameterName;
        vce = new FloatChangeEvent();
    }
}
