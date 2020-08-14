using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public float bounceSpeed = 20f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount == 0)
            return;

        ContactPoint2D contactPoint = collision.GetContact(0);
        Vector3 bounceNormal = contactPoint.normal;
        float bounceAngle = Vector2.SignedAngle(Vector2.up, bounceNormal);

        collision.rigidbody.rotation = bounceAngle;
        collision.rigidbody.position = (Vector3)contactPoint.point - bounceNormal * bounceSpeed * Time.fixedDeltaTime;
        collision.rigidbody.velocity = -bounceNormal * bounceSpeed;
        collision.rigidbody.angularVelocity = 0f;
    }
}
