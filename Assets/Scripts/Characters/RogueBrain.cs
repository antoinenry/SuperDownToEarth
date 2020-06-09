using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueBrain : MonoBehaviour
{
    public enum BrainState { Thinking, Chasing }
    public enum DetectionResult { Nothing, Avoid }

    public Transform target;
    [Min(0f)] public float thinkDelay;
    [Min(0f)] public float detectionWidth;
    [Min(0f)] public Vector2 detectionRange;
    public LayerMask detectionLayers;
    public string avoidTag = "Damaging";
    public BrainState currentState;

    private Pilot pilot;
    private float thinkTime;

    //Rework here
    public Walker walker;
    public Jumper jumper;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector2[] detectionBox = new Vector2[4]
        {
            transform.position + transform.rotation * Vector2.right * detectionWidth/2f,
            transform.position + transform.rotation * Vector2.left * detectionWidth/2f,
            transform.position + transform.rotation * Vector2.left * detectionWidth/2f + transform.rotation * Vector2.up * detectionRange.y,
            transform.position + transform.rotation * Vector2.right * detectionWidth/2f + transform.rotation * Vector2.up * detectionRange.y
        };
        
        Gizmos.DrawLine(detectionBox[0], detectionBox[3]);
        Gizmos.DrawLine(detectionBox[1], detectionBox[2]);

        Gizmos.DrawSphere(transform.position + transform.rotation * Vector2.right * detectionRange.x, .5f);
    }

    private void Awake()
    {
        pilot = GetComponent<Pilot>();
    }

    private void Start()
    {
        SwitchCoroutine();
    }

    private void SwitchCoroutine()
    {
        StopAllCoroutines();

        switch(currentState)
        {
            case BrainState.Chasing:
                StartCoroutine(ChaseCoroutine());
                break;

            case BrainState.Thinking:
                StartCoroutine(ThinkCoroutine());
                break;
        }
    }

    private IEnumerator ThinkCoroutine()
    {
        thinkTime = 0f;

        while (currentState == BrainState.Thinking)
        {
            walker.Walk(0);

            yield return new WaitForFixedUpdate();

            thinkTime += Time.fixedDeltaTime;
            if (thinkTime >= thinkDelay) currentState = BrainState.Chasing;
        }

        SwitchCoroutine();
    }

    private IEnumerator ChaseCoroutine()
    {
        while(currentState == BrainState.Chasing)
        {
            if (target != null)
            {
                float horizontalToTarget = Vector2.Dot(target.position - transform.position, transform.right);

                if (horizontalToTarget > detectionWidth / 2f && CheckRight() != DetectionResult.Avoid)
                {
                    if (walker.currentWalkDirection == -1)
                        currentState = BrainState.Thinking;
                    else
                        walker.Walk(1);
                }
                else if (horizontalToTarget < -detectionWidth / 2f && CheckLeft() != DetectionResult.Avoid)
                {
                    if (walker.currentWalkDirection == 1)
                        currentState = BrainState.Thinking;
                    else
                        walker.Walk(-1);
                }
                else
                {
                    float verticalToTarget = Vector2.Dot(target.position - transform.position, transform.up);
                    if (CheckAbove() != DetectionResult.Avoid)
                        jumper.Jump();

                    currentState = BrainState.Thinking;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        SwitchCoroutine();
    }

    private DetectionResult CheckDirection(Vector2 direction, float range, float offset = 0f)
    {
        Vector3 rayOrigin = transform.position;
        if(offset != 0f)
            rayOrigin += Quaternion.Euler(0f, 0f, 90f) * direction * offset;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, range, detectionLayers);

        if (hit)
        {
            if (hit.collider.CompareTag(avoidTag)) return DetectionResult.Avoid;
        }

        return DetectionResult.Nothing;
    }

    private DetectionResult CheckRight()
    {
        return CheckDirection(transform.rotation * Vector2.right, detectionRange.x);
    }

    private DetectionResult CheckLeft()
    {
        return CheckDirection(transform.rotation * Vector2.left, detectionRange.x);
    }

    private DetectionResult CheckAbove()
    {
        int check1 = (int)CheckDirection(transform.rotation * Vector2.up, detectionRange.y, detectionWidth / 2f);
        int check2 = (int)CheckDirection(transform.rotation * Vector2.up, detectionRange.y, -detectionWidth / 2f);

        return (DetectionResult)Mathf.Max(check1, check2);    
    }
}