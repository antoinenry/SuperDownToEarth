using System;
using UnityEngine;

[Serializable]
public class TransformState : ComponentState
{
    public Transform transform;
    public Vector3 position;
    public float rotation;

    public override Component Component => transform;
    public override Type Type => typeof(Transform);
    
    public TransformState(Transform fromTransform)
    {
        if (fromTransform == null) return;
        transform = fromTransform;
        position = fromTransform.position;
        rotation = fromTransform.rotation.eulerAngles.z;
    }
}
