using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class EventTrailRenderer : MonoBehaviour
{
    public BoolChangeEvent emitTrail;

    private TrailRenderer trail;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        emitTrail.AddValueListener<bool>(SetTrailEmission);
    }

    private void OnDisable()
    {
        emitTrail.RemoveValueListener<bool>(SetTrailEmission);
    }

    private void SetTrailEmission(bool emit)
    {
        trail.emitting = emit;
    }
}
