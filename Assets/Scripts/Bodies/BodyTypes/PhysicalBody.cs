using UnityEngine;

public class PhysicalBody : Body
{
    public Rigidbody2D rb2d;

    [Header("Physical")]
    [Header("//WEIRD PART")]
    public bool isFree;
    [Min(0f)] public float freeLinearVelocity = 1f;
    [Min(0f)] public float freeAngularVelocity = 5f;

    private bool simulate;
    private bool move;    

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
    /*
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
    */
    private void OnEnable()
    {
        SetAllFlags(true);
    }

    private void OnDisable()
    {
        SetAllFlags(false);
    }

    protected override void OnDestroyBody()
    {
        base.OnDestroyBody();
        SetAllFlags(false);
    }

    protected override void OnRespawnBody()
    {
        base.OnRespawnBody();
        SetAllFlags(true);
    }

    public void SetAllFlags(bool flagValue)
    {
        Visible = flagValue;
        Simulate = flagValue;
        Move = flagValue;
    }
}
