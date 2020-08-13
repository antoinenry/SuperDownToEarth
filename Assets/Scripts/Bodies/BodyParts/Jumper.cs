using System;
using System.Collections;
using UnityEngine;

public class Jumper : BodyPart
{
    public AnimationCurve jumpCurve;
    public float startVelocityDamping;
    public bool showGizmo;

    public Trigger tryJump;
    public Trigger jump;

    private Coroutine currentJumpCoroutine;

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
        tryJump.AddTriggerListener(OnTryJump);
        jump.AddTriggerListener(OnJump);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        currentJumpCoroutine = null;
        tryJump.RemoveTriggerListener(OnTryJump);
        jump.RemoveTriggerListener(OnJump);
    }

    public bool CanJump { get => (bool)Feet.IsOnGround.Value == true && (bool)Feet.IsTumbling.Value == false; }

    public void Jump(bool evenIfCantJump = false)
    {
        tryJump.Trigger();
        if (evenIfCantJump && CanJump == false) jump.Trigger();
    }

    private void OnTryJump()
    {
        if (CanJump) jump.Trigger();
    }

    private void OnJump()
    {
        if (currentJumpCoroutine == null)
            currentJumpCoroutine = StartCoroutine(JumpCoroutine());        
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
        bool stillOnGround = true;
        
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

            if (stillOnGround)
            {
                if ((bool)Feet.IsOnGround.Value == false) stillOnGround = false;
            }
            else
            {
                if ((bool)Feet.IsOnGround.Value == true) break;
            }
        }

        currentJumpCoroutine = null;
    }
}
