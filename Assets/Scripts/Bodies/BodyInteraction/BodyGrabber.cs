using UnityEngine;

public class BodyGrabber : MonoBehaviour
{    
    public Transform target;
    public Trigger grabTarget;

    private void OnEnable()
    {
        grabTarget.AddTriggerListener(OnGrabTarget);
    }

    private void OnDisable()
    {
        grabTarget.RemoveTriggerListener(OnGrabTarget);
    }

    private void OnGrabTarget()
    {
        if (target == null) return;

        PhysicalBody body = target.GetComponent<PhysicalBody>();
        if (body != null)
        {
            body.Move = false;
        }

        target.position = transform.position;
        target.rotation = transform.rotation;
    }

    public void GrabTarget()
    {
        grabTarget.Trigger();
    }
}
