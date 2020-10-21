using UnityEngine;

public class Hider : BodyPart
{
    public BoolChangeEvent isHiding;
    public Trigger stopHiding;
    public Trigger tryFunnel;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
    }

    private void OnEnable()
    {
        isHiding.AddValueListener<bool>(OnHide, false);
    }

    private void OnDisable()
    {
        isHiding.RemoveValueListener<bool>(OnHide);
    }

    public void Hide()
    {
        isHiding.Value = true;
    }

    public void StopHiding(Vector2 jumpOutVelocity)
    {
        stopHiding.Trigger();
        if (AttachedRigidbody != null)
            AttachedRigidbody.velocity = jumpOutVelocity;
    }

    public void StopHiding()
    {
        StopHiding(Vector2.zero);
    }

    private void OnHide(bool hide)
    {
        if (hide)
            stopHiding.AddTriggerListener(OnStopHiding, false);
        else
            stopHiding.RemoveTriggerListener(OnStopHiding);

        if (AttachedBody != null)
        {
            AttachedBody.Visible = !hide;

            if (AttachedBody is PhysicalBody)
            {
                PhysicalBody physical = AttachedBody as PhysicalBody;
                physical.Simulate = !hide;
                physical.Move = !hide;
            }
        }
    }

    private void OnStopHiding()
    {
        isHiding.Value = false;
    }
}
