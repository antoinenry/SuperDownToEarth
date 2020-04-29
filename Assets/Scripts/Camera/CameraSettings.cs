using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public struct CameraSettingsValues
{
    public Transform target;
    public Vector2 positionOffset;
    public float orthographicSize;
}

[System.Serializable]
public struct CameraSettings
{
    public enum ActionType { UseDefault, UseValue, DoNothing }
    
    [System.Serializable]
    public struct FloatSetting
    {
        public ActionType action;
        public float value;
    }

    [System.Serializable]
    public struct Vector2Setting
    {
        public ActionType action;
        public Vector2 value;
    }

    [System.Serializable]
    public struct TransformSetting
    {
        public ActionType action;
        public Transform value;
    }

    public TransformSetting target;
    public Vector2Setting positionOffset;
    public FloatSetting orthographicSize;

    public CameraSettings(CameraSettingsValues values)
    {
        target.action = ActionType.UseValue;
        target.value = values.target;

        positionOffset.action = ActionType.UseValue;
        positionOffset.value = values.positionOffset;

        orthographicSize.action = ActionType.UseValue;
        orthographicSize.value = values.orthographicSize;
    }
}
