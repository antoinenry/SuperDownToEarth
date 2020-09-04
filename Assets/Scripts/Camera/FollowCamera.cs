using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public bool followX = true;
    public bool followY = true;
    public float damping = 10f;
    public Rect travelingRect;
    public Rect neutralRect;

    private Camera thisCamera;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawLine(travelingRect.min, new Vector2(travelingRect.xMin, travelingRect.yMax));
        Gizmos.DrawLine(new Vector2(travelingRect.xMin, travelingRect.yMax), travelingRect.max);
        Gizmos.DrawLine(travelingRect.max, new Vector2(travelingRect.xMax, travelingRect.yMin));
        Gizmos.DrawLine(new Vector2(travelingRect.xMax, travelingRect.yMin), travelingRect.min);

        Rect neutralBounds = GetNeutralBounds();
        Gizmos.DrawLine(neutralBounds.min, new Vector2(neutralBounds.xMin, neutralBounds.yMax));
        Gizmos.DrawLine(new Vector2(neutralBounds.xMin, neutralBounds.yMax), neutralBounds.max);
        Gizmos.DrawLine(neutralBounds.max, new Vector2(neutralBounds.xMax, neutralBounds.yMin));
        Gizmos.DrawLine(new Vector2(neutralBounds.xMax, neutralBounds.yMin), neutralBounds.min);
    }

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Rect neutralBounds = GetNeutralBounds();
        if (target != null)
        {
            bool neutralX = target.position.x >= neutralBounds.xMin && target.position.x <= neutralBounds.xMax;
            bool neutralY = target.position.y >= neutralBounds.yMin && target.position.y <= neutralBounds.yMax;
            Follow(target.position, neutralX, neutralY);
        }        
    }

    public Rect GetTravelingBounds()
    {
        Rect bounds = travelingRect;
        if (thisCamera != null)
        {
            bounds.yMin += thisCamera.orthographicSize;
            bounds.yMax -= thisCamera.orthographicSize;
            bounds.xMin += thisCamera.orthographicSize * thisCamera.aspect;
            bounds.xMax -= thisCamera.orthographicSize * thisCamera.aspect;
        }

        return bounds;
    }

    public Rect GetNeutralBounds()
    {
        Rect bounds = neutralRect;
        bounds.position += (Vector2)transform.position;
        return bounds;
    }

    private void Follow(Vector2 targetPosition, bool ignoreX, bool ignoreY)
    {
        Rect travelingBounds = GetTravelingBounds();
        Vector3 moveTo = new Vector2();
        moveTo.x = !ignoreX && followX ? Mathf.Clamp(targetPosition.x, travelingBounds.xMin, travelingBounds.xMax) : transform.position.x;
        moveTo.y = !ignoreY && followY ? Mathf.Clamp(targetPosition.y, travelingBounds.yMin, travelingBounds.yMax) : transform.position.y;
        moveTo.z = transform.position.z;

        if (damping > 0f)
            transform.position = Vector3.Lerp(transform.position, moveTo, 1f / damping);
        else
            transform.position = moveTo;
    }
}
