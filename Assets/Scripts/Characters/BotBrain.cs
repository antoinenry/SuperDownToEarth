using UnityEngine;
using System.Collections;

public class BotBrain : MonoBehaviour
{
    public enum BrainState { Thinking, Chasing }
    public enum DetectionResult { Nothing, Avoid }

    public BrainState currentState;
    public PhysicalBody body;
    public Transform followTransform;
    public Transform avoidTransform;
    [Min(0f)] public float thinkDelay;
    [Min(0f)] public float detectionWidth;
    [Min(0f)] public Vector2 detectionRange;
    public float avoidDistance = 15f;
    public LayerMask detectionLayers;
    public string followTag = "Player";
    public string avoidTag = "Damaging";
    [Range(0f, 1f)] public float jumpiness = .5f;
    [Range(0f, 1f)] public float randomness = .5f;
    public float spinDuration = 1f;
    
    private Walker walker;
    private Jumper jumper;
    private Spinner spinner;
    private Feet feet;
    private StickToSurface stick;
    private Pilot player;

    private float thinkTime;
    private Coroutine currentStateCoroutine;

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
        walker = body.GetComponent<Walker>();
        jumper = body.GetComponent<Jumper>();
        spinner = body.GetComponent<Spinner>();
        feet = body.GetComponent<Feet>();
        stick = body.GetComponent<StickToSurface>();

        GameObject playerGO = GameObject.FindWithTag(followTag);
        if (playerGO != null) player = playerGO.GetComponentInChildren<Pilot>(true);
    }

    private void OnEnable()
    {
        StateCoroutineSwitch();
        feet.IsOnGround.AddValueListener<bool>(OnTouchGround);
        jumper.jump.AddTriggerListener(OnJump);
        player.isPilotingVehicle.AddValueListener<bool>(OnPlayerIsPiloting);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        currentStateCoroutine = null;
        feet.IsOnGround.RemoveValueListener<bool>(OnTouchGround);
        jumper.jump.RemoveTriggerListener(OnJump);
        player.isPilotingVehicle.RemoveValueListener<bool>(OnPlayerIsPiloting);
    }

    private void StateCoroutineSwitch()
    {
        if (currentStateCoroutine != null)
        {
            StopCoroutine(currentStateCoroutine);
            currentStateCoroutine = null;

        }

        switch (currentState)
        {
            case BrainState.Chasing:
                currentStateCoroutine = StartCoroutine(ChaseCoroutine());
                break;

            case BrainState.Thinking:
                currentStateCoroutine = StartCoroutine(ThinkCoroutine());
                break;

            default:
                currentStateCoroutine = null;
                break;
        }
    }

    private IEnumerator ThinkCoroutine()
    {
        thinkTime = thinkDelay * Random.Range(-randomness, randomness);

        while (currentState == BrainState.Thinking)
        {
            walker.Walk(0);

            yield return new WaitForFixedUpdate();

            thinkTime += Time.fixedDeltaTime;
            if (thinkTime >= thinkDelay)
            {
                if (followTransform != null)
                    currentState = BrainState.Chasing;
                else
                    thinkTime = thinkDelay * Random.Range(-randomness, randomness);
            }
        }
        
        StateCoroutineSwitch();
    }

    private IEnumerator ChaseCoroutine()
    {
        while (currentState == BrainState.Chasing)
        {
            if (followTransform != null)
            {
                float horizontalToTarget = Vector2.Dot(followTransform.position - transform.position, transform.right);

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
                    if (CheckAbove() != DetectionResult.Avoid)
                        jumper.Jump();

                    currentState = BrainState.Thinking;
                }
            }
            else
                currentState = BrainState.Thinking;

            yield return new WaitForFixedUpdate();
        }

        StateCoroutineSwitch();
    }
    
    private void OnJump()
    {
        stick.enabled = false;
        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        float spinTimer = 0f;

        while (spinTimer < spinDuration)
        {
            spinner.Spin(1);

            yield return new WaitForFixedUpdate();
            spinTimer += Time.fixedDeltaTime;
        }

        spinner.Spin(0);

        currentState = BrainState.Thinking;
        StateCoroutineSwitch();
    }

    private void OnTouchGround(bool touch)
    {
        if (touch) stick.enabled = true;
    }

    private void OnPlayerIsPiloting(bool piloting)
    {
        if (piloting)
        {
            followTransform = null;
            avoidTransform = player.transform;
        }
        else
        {
            followTransform = player.transform;
            avoidTransform = null;
        }
    }

    private DetectionResult CheckDirection(Vector2 direction, float range, float offset = 0f)
    {
        Vector3 rayOrigin = transform.position;
        if (offset != 0f)
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
