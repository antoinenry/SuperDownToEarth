using System.Collections;
using UnityEngine;

public class Propeller : BodyPart
{
    public float populsionForce = 100f;
    public float propulsionAngle = 90f;
    public bool constantWorldDirection = false;
    public float propulsionDuration = -1f;
    public bool automatic = false;
    
    public BoolChangeEvent running;

    private Coroutine propulsionCoroutine;
    private bool colliding;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
    }

    private void OnEnable()
    {
        running.AddValueListener<bool>(OnSetRunning);
    }

    private void OnDisable()
    {
        running.RemoveValueListener<bool>(OnSetRunning);
    }

    private void FixedUpdate()
    {
        if (automatic == true)
            running.Value = !colliding;        
        colliding = false;
    }

    public Vector2 PropulsionDirection
        => constantWorldDirection ?
            Quaternion.Euler(0f, 0f, propulsionAngle) * Vector2.right :
            Quaternion.Euler(0f, 0f, propulsionAngle) * transform.rotation * Vector2.right;

    public void Run()
    {
        running.Value = true;
    }

    public void Stop()
    {
        running.Value = false;
    }


    private void OnSetRunning(bool on)
    {
        if (on)
        {
            if (propulsionCoroutine == null)
                propulsionCoroutine = StartCoroutine(PropulsionCoroutine());
        }
        else
        {
            if (propulsionCoroutine != null)
            {
                StopCoroutine(propulsionCoroutine);
                propulsionCoroutine = null;
            }
        }
    }

    private IEnumerator PropulsionCoroutine()
    {
        float propulsionTimer = 0f;

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
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        colliding = true;
    }
}
