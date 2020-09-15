using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Serializable]
    public struct FollowSettings
    {
        public Transform target;
        public bool followX;
        public bool followY;
        public Vector2 targetPosition;
        public float travelDamping;
        public Rect travelingRect;
        public Vector2 neutralZoneSize;
        public float orthographicSize;
        public float zoomDamping;
    }

    public FollowSettings currentSettings;
    private Camera thisCamera;

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Rect neutralBounds = GetNeutralBounds();
        if (currentSettings.target != null)
        {
            if (currentSettings.followX) currentSettings.targetPosition.x = currentSettings.target.position.x;
            if (currentSettings.followY) currentSettings.targetPosition.y = currentSettings.target.position.y;

            bool neutralX = currentSettings.targetPosition.x >= neutralBounds.xMin && currentSettings.targetPosition.x <= neutralBounds.xMax;
            bool neutralY = currentSettings.targetPosition.y >= neutralBounds.yMin && currentSettings.targetPosition.y <= neutralBounds.yMax;
            Follow(currentSettings.targetPosition, neutralX, neutralY);
        }        
    }
    
    public Rect GetTravelingBounds()
    {
        Rect bounds = currentSettings.travelingRect;
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
        Rect bounds = new Rect(transform.position, currentSettings.neutralZoneSize);
        return bounds;
    }


    public void Snap()
    {
        Vector3 pos3 = currentSettings.targetPosition;
        pos3.z = transform.position.z;
        transform.position = pos3;

        if (thisCamera == null) thisCamera = GetComponent<Camera>();
        thisCamera.orthographicSize = currentSettings.orthographicSize;
    }

    public void Follow(Vector2 targetPosition, bool ignoreX, bool ignoreY)
    {
        Rect travelingBounds = GetTravelingBounds();
        Vector3 moveTo = new Vector2();
        moveTo.x = !ignoreX ? Mathf.Clamp(targetPosition.x, travelingBounds.xMin, travelingBounds.xMax) : transform.position.x;
        moveTo.y = !ignoreY ? Mathf.Clamp(targetPosition.y, travelingBounds.yMin, travelingBounds.yMax) : transform.position.y;
        moveTo.z = transform.position.z;

        if (currentSettings.travelDamping > 0f)
        {
            transform.position = Vector3.Lerp(transform.position, moveTo, 1f / currentSettings.travelDamping);
            thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, currentSettings.orthographicSize, 1f / currentSettings.zoomDamping);
        }
        else
        {
            transform.position = moveTo;
            thisCamera.orthographicSize = currentSettings.orthographicSize;
        }
    }
}
