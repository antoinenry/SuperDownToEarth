using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public Transform movingPart;

    [Header("Openning")]
    public Vector2 openPosition;
    public float openRotation;
    public float openDuration;

    [Header("Closing")]
    public Vector2 closedPosition;
    public float closedRotation;
    public float closeDuration;

    public BoolChangeEvent isOpen = new BoolChangeEvent();

    private Coroutine currentMoveCoroutine;
    private Rigidbody2D rb2d;

    public bool IsInitialized { get; private set; }
    public bool IsMoving => currentMoveCoroutine != null;

    private void Awake()
    {
        if (movingPart != null)
            rb2d = movingPart.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        isOpen.AddValueListener<bool>(OnOpen);
    }

    private void OnDisable()
    {
        isOpen.RemoveValueListener<bool>(OnOpen);
    }

    private void Reset()
    {
        movingPart = transform;

        openPosition = transform.localPosition;
        openRotation = transform.localRotation.eulerAngles.z;
    
        closedPosition = transform.localPosition;
        closedRotation = transform.localRotation.eulerAngles.z;
    }

    private void OnOpen(bool open)
    {
        if (IsMoving)
            StopCoroutine(currentMoveCoroutine);

        if (rb2d != null)
        {
            if (open)
                currentMoveCoroutine = StartCoroutine(MoveBodyCoroutine(openPosition, openRotation, openDuration));
            else
                currentMoveCoroutine = StartCoroutine(MoveBodyCoroutine(closedPosition, closedRotation, closeDuration));
        }
        else if (movingPart != null)
        {
            if (open)
                currentMoveCoroutine = StartCoroutine(MoveTransformCoroutine(openPosition, openRotation, openDuration));
            else
                currentMoveCoroutine = StartCoroutine(MoveTransformCoroutine(closedPosition, closedRotation, closeDuration));
        }
    }

    private IEnumerator MoveBodyCoroutine(Vector2 toLocalPosition, float toLocalRotation, float duration)
    {
        Vector2 toPosition = movingPart.parent == null ? toLocalPosition : (Vector2)(movingPart.parent.rotation * toLocalPosition) + (Vector2)movingPart.parent.position;
        float toRotation = movingPart.parent == null ? toLocalRotation : movingPart.parent.rotation.eulerAngles.z + toLocalRotation;
        float routineTimer = 0f;

        if (duration > 0f)
        {
            Vector2 linearVelocity = (toPosition - rb2d.position) / duration;
            float angularVelocity = Mathf.DeltaAngle(rb2d.rotation, toRotation) / duration;

            while (routineTimer < duration)
            {
                rb2d.velocity = linearVelocity;
                rb2d.angularVelocity = angularVelocity;

                yield return new WaitForFixedUpdate();
                routineTimer += Time.fixedDeltaTime;
            }
        }        

        rb2d.MovePosition(toPosition);
        rb2d.velocity = Vector2.zero;
        rb2d.MoveRotation(toRotation);
        rb2d.angularVelocity = 0f;

        currentMoveCoroutine = null;
    }

    private IEnumerator MoveTransformCoroutine(Vector2 toLocalPosition, float toLocalRotation, float duration)
    {
        Vector2 toPosition = movingPart.parent == null ? toLocalPosition : toLocalPosition + (Vector2)movingPart.parent.position;
        float toRotation = movingPart.parent == null ? toLocalRotation : toLocalRotation + movingPart.parent.rotation.eulerAngles.z;
        float routineTimer = 0f;

        if (duration > 0f)
        {
            Vector3 linearVelocity = (toPosition - (Vector2)movingPart.position) / duration;
            float angularVelocity = Mathf.DeltaAngle(movingPart.rotation.eulerAngles.z, toRotation) / duration;

            while (routineTimer < duration)
            {
                float deltaTime = Time.fixedDeltaTime;
                movingPart.position += linearVelocity * deltaTime;
                movingPart.rotation = Quaternion.Euler(0f, 0f, movingPart.rotation.eulerAngles.z + angularVelocity * deltaTime);

                yield return new WaitForFixedUpdate();
                routineTimer += Time.fixedDeltaTime;
            }
        }

        movingPart.position = toPosition;
        movingPart.rotation = Quaternion.Euler(0f, 0f, toRotation);

        currentMoveCoroutine = null;
    }
}
