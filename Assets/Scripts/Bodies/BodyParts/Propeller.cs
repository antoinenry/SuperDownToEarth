using System.Collections;
using UnityEngine;

public class Propeller : BodyPart
{
    public float populsionForce = 100f;
    public float propulsionAngle = 90f;
    public bool constantWorldDirection = false;
    public float propulsionDuration = -1f;
    public bool oneShot = false;
    public bool cancelVelocity = false;
    public bool feetEnabled = false;

    public Trigger run;
    public Trigger stop;
    public BoolChangeEvent running;

    private Coroutine propulsionCoroutine;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
    }

    private void Start()
    {
        if (feetEnabled)
        {
            Feet feet = GetComponent<Feet>();
            feet.IsOnGround.AddValueListener<bool>(OnFeetTouchGround);
        }
    }

    private void OnDestroy()
    {
        if (feetEnabled)
        {
            Feet feet = GetComponent<Feet>();
            feet.IsOnGround.AddValueListener<bool>(OnFeetTouchGround);
        }
    }

    private void OnEnable()
    {
        if (AttachedRigidbody.simulated == false) enabled = false;

        run.AddTriggerListener(Run);
        stop.AddTriggerListener(Stop);
        running.AddValueListener<bool>(OnSetRunning);
    }

    private void OnDisable()
    {
        Stop();

        run.RemoveTriggerListener(Run);
        stop.RemoveTriggerListener(Stop);
        running.RemoveValueListener<bool>(OnSetRunning);
    }

    private void OnFeetTouchGround(bool isOnGround)
    {
        enabled = !isOnGround;
    }

    public Vector2 PropulsionDirection
        => constantWorldDirection ?
            Quaternion.Euler(0f, 0f, propulsionAngle) * Vector2.right :
            Quaternion.Euler(0f, 0f, propulsionAngle) * transform.rotation * Vector2.right;

    public void Run()
    {
        if (enabled)
            running.Value = true;
    }

    public void Stop()
    {
        if (enabled)
            running.Value = false;
    }


    private void OnSetRunning(bool on)
    {
        if (on)
        {
            if (propulsionCoroutine == null)
            {
                run.Trigger();
                propulsionCoroutine = StartCoroutine(PropulsionCoroutine());
            }
        }
        else
        {
            if (propulsionCoroutine != null)
            {
                stop.Trigger();
                StopCoroutine(propulsionCoroutine);
                propulsionCoroutine = null;
            }
        }
    }

    private IEnumerator PropulsionCoroutine()
    {
        float propulsionTimer = 0f;

        if (cancelVelocity)
        {
            AttachedRigidbody.velocity = Vector2.zero;
            AttachedRigidbody.angularVelocity = 0f;
        }

        while (running)
        {
            AttachedRigidbody.AddForce(PropulsionDirection * populsionForce);
            yield return new WaitForFixedUpdate();

            if (propulsionDuration > 0f)
            {
                propulsionTimer += Time.fixedDeltaTime;
                if (propulsionTimer > propulsionDuration) running.Value = false;
            }
            else if (propulsionDuration == 0f) running.Value = false;
        }

        propulsionCoroutine = null;
        if (oneShot) enabled = false;
    }
}
