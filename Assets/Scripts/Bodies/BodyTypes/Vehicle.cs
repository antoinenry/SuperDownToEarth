using UnityEngine;

public class Vehicle : PhysicalBody
{
    [Header("Vehicle")]
    public Transform seat;
    public Transform exit;
    public float exitForce;
    public float exitAnimationDelay;

    public Trigger eject;

    public BoolChangeEvent IsFull;
    public Body BodyInside { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SetBodyInside(BodyInside);
    }

    protected override void Start()
    {
        base.Start();
        eject.AddTriggerListener(Eject);
    }

    public void Eject()
    {
        SetBodyInside(null);
    }

    public void SetBodyInside(Body body)
    {
        BodyInside = body;

        if (body == null)
        {
            IsFull.Value = false;
            isFree = true;
        }
        else
        {
            IsFull.Value = true;
            isFree = false;
        }
    }
}