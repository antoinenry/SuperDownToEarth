using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class CircuitTraveler : MonoBehaviour
{
    public Circuit circuit;
    [Min(0f)] public float speed;
    public Rigidbody2D rb2D;
    public IntChangeEvent currentDestinationIndex;
    public BoolChangeEvent destinationReached;

    private Vector2 currentDestinationPosition;
    private Coroutine currentMoveCoroutine;

    public bool IsMoving
    {
        get => currentMoveCoroutine != null;
        set
        {
            if(value == true)
            {
                if (IsMoving) StopCoroutine(currentMoveCoroutine);
                currentMoveCoroutine = StartCoroutine(MoveCoroutine());
            }
            else
            {
                if (IsMoving) StopCoroutine(currentMoveCoroutine);
                currentMoveCoroutine = null;
            }
        }
    }

    private void OnEnable()
    {
        currentDestinationIndex.AddValueListener<int>(OnCurrentDestinationChange);
        destinationReached.AddValueListener<bool>(OnDestinationReached);
    }

    private void OnDisable()
    {
        currentDestinationIndex.RemoveValueListener<int>(OnCurrentDestinationChange);
        destinationReached.RemoveValueListener<bool>(OnDestinationReached);
    }

    private IEnumerator MoveCoroutine()
    {
        while (destinationReached == false)
        {
            Vector3 newPosition = Vector2.MoveTowards(transform.position, currentDestinationPosition, Time.fixedDeltaTime * speed);
            newPosition.z = transform.position.z;
            transform.position = newPosition;

            yield return new WaitForFixedUpdate();
        }

        currentMoveCoroutine = null;
    }

    private void OnCurrentDestinationChange(int destinationIndex)
    {
        if (circuit != null)
        {
            Vector2? pos = circuit.GetPosition(destinationIndex);
            if (pos != null)
            {
                currentDestinationPosition = pos.Value;
                destinationReached.Value = false;
            }
        }
    }

    private void OnDestinationReached(bool reached)
    {
        if (Application.isPlaying)
        {
            IsMoving = !reached;
            if (reached)
                SetPosition(currentDestinationPosition, true);
        }
        else
        {
            if ((Vector2)transform.position != currentDestinationPosition)
            {
                SetPosition(currentDestinationPosition, false);
                destinationReached.Value = true;
            }
        }
    }

    private void SetPosition(Vector2 pos, bool setVelocity)
    {
        if (Application.isPlaying == false || rb2D == null)
        {
            Vector3 pos3 = pos;
            pos3.z = transform.position.z;
            transform.position = pos;
        }
        else
        {
            if (setVelocity)
                rb2D.velocity = (pos - rb2D.position) / Time.fixedDeltaTime;
            rb2D.MovePosition(pos);
        }
    }
}
