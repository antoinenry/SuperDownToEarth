using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Swimmer : BodyPart
{
    public string floatingTag = "Water";
    [Min(0f)] public float floatSpring = 1f;
    [Min(0f)] public float maxFloatForce = 10f;

    public BoolChangeEvent IsInFluid;
    
    private Vector2 surfacePoint;
    private Collider2D currentFluid;

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && IsInFluid)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, surfacePoint);
        }
    }

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
    }    

    private void FixedUpdate()
    {
        if (IsInFluid && currentFluid != null)
        {
            surfacePoint = AttachedRigidbody.Distance(currentFluid).pointB;
            AttachedRigidbody.AddForce(FloatForce());
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(floatingTag))
        {
            Debug.Break();
            //surfacePoint = collision.ClosestPoint(AttachedRigidbody.position);
            currentFluid = collision;
            IsInFluid.Value = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == currentFluid)
        {
            currentFluid = null;
            IsInFluid.Value = false;
        }
    }
    
    private Vector2 FloatForce()
    {
        //Vector2 force = Vector2.ClampMagnitude((surfacePoint - AttachedRigidbody.position) * floatSpring, maxFloatForce);
        
        surfacePoint = currentFluid.transform.position;
        Vector2 force = (AttachedRigidbody.position - surfacePoint) * maxFloatForce;
        return force;
    }
}
