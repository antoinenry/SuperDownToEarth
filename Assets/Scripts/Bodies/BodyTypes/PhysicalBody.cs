using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalBody : Body
{
    public Rigidbody2D rb2d;
    public SpriteRenderer render;

    [Header("Physical")]
    public bool isFree;
    [Min(0f)] public float freeLinearVelocity = 1f;
    [Min(0f)] public float freeAngularVelocity = 5f;

    private bool visible;
    private bool simulate;
    private bool move;

    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            if (render != null) render.enabled = value;
        }
    }

    public bool Simulate
    {
        get => simulate;
        set
        {
            simulate = value;
            if (rb2d != null)
            {
                if (value == false)
                {
                    rb2d.velocity = Vector2.zero;
                    rb2d.angularVelocity = 0f;
                }
                rb2d.simulated = value;
            }
        }
    }

    public bool Move
    {
        get => move;
        set
        {
            move = value;
            if (rb2d != null) rb2d.constraints = value ? RigidbodyConstraints2D.None : RigidbodyConstraints2D.FreezeAll;
        }
    }

    public override Rigidbody2D AttachedRigidBody
    {
        get => rb2d;
        protected set => rb2d = value;
    }

    public override SpriteRenderer AttachedRenderer
    {
        get => render;
        protected set => render = value;
    }

    private void FixedUpdate()
    {
        if(isFree)
        {
            if(rb2d.velocity.sqrMagnitude < freeLinearVelocity * freeLinearVelocity)
            {
                if (rb2d.velocity != Vector2.zero)
                    rb2d.velocity = rb2d.velocity.normalized * freeLinearVelocity;
                else
                    rb2d.velocity = -transform.up * freeLinearVelocity;
            }

            if (Mathf.Abs(rb2d.angularVelocity) < freeAngularVelocity)
            {
                if (rb2d.angularVelocity > 0f)
                    rb2d.angularVelocity = freeAngularVelocity;
                else
                    rb2d.angularVelocity = -freeAngularVelocity;
            }
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SetAllFlags(true);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SetAllFlags(false);
    }

    public override void Kill()
    {
        base.Kill();
        SetAllFlags(false);
    }

    public override void Respawn()
    {
        base.Respawn();
        SetAllFlags(true);
    }

    public void SetAllFlags(bool flagValue)
    {
        Visible = flagValue;
        Simulate = flagValue;
        Move = flagValue;
    }
}
