using UnityEngine;

public class AimCursor : MonoBehaviour
{
    public float minDistance = 1f;
    public Vector2ChangeEvent aimPosition;
    public LayerMask collisionLayers;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        aimPosition.AddValueListener<Vector2>(OnPositionChange);
    }

    private void OnDisable()
    {
        aimPosition.RemoveValueListener<Vector2>(OnPositionChange);
    }

    private void OnPositionChange(Vector2 newPos)
    {
        float distance = newPos.magnitude;
        if (distance >= minDistance)
        {
            line.enabled = true;
            DrawRay(newPos, distance);
        }
        else
            line.enabled = false;
    }

    private void DrawRay(Vector2 direction, float maxLength)
    {
        line.SetPosition(1, Quaternion.Inverse(transform.rotation) * direction.normalized * minDistance);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxLength, collisionLayers);
        if (hit)
            line.SetPosition(0, Quaternion.Inverse(transform.rotation) * Vector2.Scale(hit.point - (Vector2)transform.position, transform.lossyScale));
        else
            line.SetPosition(0, Quaternion.Inverse(transform.rotation) * Vector2.Scale(direction, transform.lossyScale));
    }
}
