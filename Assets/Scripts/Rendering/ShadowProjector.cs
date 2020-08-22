using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowProjector : MonoBehaviour
{
    public Transform shadowObject;
    public LayerMask projectionLayers;
    public Vector2 closeShadowScale = Vector3.one;
    public float maxDistance = 10f;
    
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, maxDistance, projectionLayers);

        if (hit.collider == null)
        {
            shadowObject.localScale = Vector3.zero;
        }
        else
        {
            shadowObject.position = hit.point;
            shadowObject.rotation = ShadowRotation(hit.normal);
            shadowObject.localScale = ShadowScale(Vector2.Distance(hit.point, transform.position));
        }
    }

    private Vector3 ShadowScale(float projectionDistance)
    {        
        float scaleFactor = 0f;

        if (projectionDistance < maxDistance)
            scaleFactor = (maxDistance - projectionDistance) / maxDistance;

        return scaleFactor * closeShadowScale;
    }

    private Quaternion ShadowRotation(Vector2 normal)
    {
        return Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, normal));
    }
}
