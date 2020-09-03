using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public bool followX = true;
    public bool followY = true;
    public float damping = 10f;
    public Vector2 topLeftBound;
    public Vector2 bottomRightBound;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(topLeftBound, new Vector2(bottomRightBound.x, topLeftBound.y));
        Gizmos.DrawLine(new Vector2(bottomRightBound.x, topLeftBound.y), bottomRightBound);
        Gizmos.DrawLine(bottomRightBound, new Vector2(topLeftBound.x, bottomRightBound.y));
        Gizmos.DrawLine(new Vector2(topLeftBound.x, bottomRightBound.y), topLeftBound);
    }

    private void Update()
    {
        Vector3 moveTo = new Vector2();
        moveTo.x = followX ? Mathf.Clamp(target.position.x, topLeftBound.x, bottomRightBound.x) : transform.position.x;
        moveTo.y = followY ? Mathf.Clamp(target.position.y, bottomRightBound.y, topLeftBound.y) : transform.position.y;
        moveTo.z = transform.position.z;

        if (damping > 0f)
            transform.position = Vector3.Lerp(transform.position, moveTo, 1f / damping);
        else
            transform.position = moveTo;
    }
}
