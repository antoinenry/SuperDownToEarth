using System;
using System.Collections;
using UnityEngine;

public class Jumper : BodyPart
{
    public AnimationCurve jumpCurve;
    public bool airJump;
    public float startVelocityDamping;
    public bool showGizmo;

    public Trigger jump;

    public Feet Feet { get; private set; }    

    private void OnDrawGizmos()
    {
        if(showGizmo)
        {
            Gizmos.color = Color.grey;

            Vector3 pt = transform.position;
            Vector3 jumpDirection = transform.rotation * Vector2.up;

            foreach (Keyframe kf in jumpCurve.keys)
            {
                Vector3 nextPt = transform.position + jumpDirection * kf.value;
                Gizmos.DrawLine(pt, nextPt);
                pt = nextPt;
            }
        }
    }

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
        Feet = GetComponent<Feet>();
    }

    private void OnEnable()
    {
        jump.AddTriggerListener(OnJump);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        jump.RemoveTriggerListener(OnJump);
    }

    public void Jump()
    {
        jump.Trigger();
    }

    private void OnJump()
    {
        if ((airJump || (bool)Feet.IsOnGround.Value == true) && (bool)Feet.IsTumbling.Value == false)
        {
            StartCoroutine(JumpCoroutine());
        }
    }

    private IEnumerator JumpCoroutine()
    {
        Vector2 jumpDirection = Quaternion.Euler(0f, 0f, AttachedRigidbody.rotation) * Vector2.up;
        Vector2 startPosition = AttachedRigidbody.position;
        
        float currentTime = 0f;
        Vector2 currentPosition = startPosition;
        Vector2 lastPosition = startPosition;
        float jumpDuration = jumpCurve.keys[jumpCurve.length - 1].time;
        Vector2 additionnalVelocity = Feet.GroundVelocity;
        bool onGround = true;
        
        while (AttachedRigidbody.simulated == true)
        {
            float deltaTime = Time.fixedDeltaTime;
            additionnalVelocity = Vector2.MoveTowards(additionnalVelocity, Vector2.zero, startVelocityDamping * deltaTime);
            currentTime += deltaTime;

            if (currentTime >= jumpDuration)
            {
                AttachedRigidbody.velocity = (currentPosition - lastPosition) / deltaTime;
                break;
            }
            
            float currentHeight = jumpCurve.Evaluate(currentTime);
            lastPosition = currentPosition;
            currentPosition = startPosition + jumpDirection * currentHeight + additionnalVelocity * currentTime;
            AttachedRigidbody.MovePosition(currentPosition);

            yield return new WaitForFixedUpdate();

            if ((bool)Feet.IsTumbling.Value == true) break;

            if (onGround)
            {
                if ((bool)Feet.IsOnGround.Value == false) onGround = false;
            }
            else
            {
                if ((bool)Feet.IsOnGround.Value == true) break;
            }
        }
    }
}
