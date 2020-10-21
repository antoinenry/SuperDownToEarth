using UnityEngine;
using System.Collections;

public class BotVehicle : Vehicle
{
    [Header("Bot parameters")]

    public float minHitVelocity = 10f;
    public float backHitBounce = 1f;
    public float backHitTorque = 50f;
    public float backHitDuration = 2f;

    private Feet botFeet;
    private LocalGravity botGravity;
    private Coroutine currentBackHitCoroutine;

    protected override void Awake()
    {
        base.Awake();
        botFeet = GetComponent<Feet>();
        botGravity = GetComponent<LocalGravity>();
    }

    public override void SetBodyInside(Body body)
    {
        base.SetBodyInside(body);

        LocalGravity gravity = GetComponent<LocalGravity>();
        if (gravity == null) return;

        if (body == null)
            gravity.enabled = false;
        else
            gravity.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentBackHitCoroutine == null && IsFull && botFeet.IsTumbling == false)
        {
            Rigidbody2D hittingRB = collision.attachedRigidbody;
            if (hittingRB != null)
            {
                /* //Version "back only"
                float flipX = transform.lossyScale.x > 0f ? 1f : -1f;
                float positionDelta = Vector2.Dot(collision.transform.position - transform.position, transform.right);
                if (flipX * positionDelta < 0f)
                {
                    float velocityDelta = Vector2.Dot(collision.attachedRigidbody.velocity, transform.right);
                    if (flipX * velocityDelta > minHitVelocity)
                    {
                        currentBackHitCoroutine = StartCoroutine(BackHitCoroutine());
                    }
                }
                */

                // Version front&back
                float horizontalVelocity = Vector2.Dot(collision.attachedRigidbody.velocity, transform.right);
                if (Mathf.Abs(horizontalVelocity) > minHitVelocity)
                {
                    currentBackHitCoroutine = StartCoroutine(BackHitCoroutine());
                }
            }
        }
    }

    private IEnumerator BackHitCoroutine()
    {
        float backHitTimer = 0f;
        Vector2 bounceDirection = transform.up;

        botGravity.enabled = false;

        while(backHitTimer < backHitDuration)
        {
            botFeet.IsTumbling.Value = true;

            AttachedRigidBody.AddTorque(-backHitTorque);
            AttachedRigidBody.AddForce(backHitBounce * bounceDirection);

            yield return new WaitForFixedUpdate();
            backHitTimer += Time.fixedDeltaTime;
        }

        //Eject();
        botGravity.enabled = true;
        currentBackHitCoroutine = null;
    }
}
