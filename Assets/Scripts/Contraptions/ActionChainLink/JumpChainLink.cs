using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class JumpChainLink : ActionChainLink
{
    public SpriteRenderer sr;
    public BoolChangeEvent isCurrentAndActivated;

    private bool listenToCollision;
    private Feet activator;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isCurrentAndActivated.AddValueListener<bool>(OnCurrentAndActivated, false);
        listenToCollision = isFirstLink;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isCurrentAndActivated.RemoveValueListener<bool>(OnCurrentAndActivated);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (listenToCollision)
        {
            Feet incoming = null;
            if (collision.attachedRigidbody != null)
                incoming = collision.attachedRigidbody.GetComponent<Feet>();
            if (incoming != null) SetActivator(incoming);
        }      
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (listenToCollision)
        {
            Feet leaving = null;
            if (collision.attachedRigidbody != null)
                leaving = collision.attachedRigidbody.GetComponent<Feet>();
            if (leaving != null && leaving == activator) SetActivator(null);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (listenToCollision)
        {
            Feet incoming = null;
            if (collision.collider != null)
                incoming = collision.collider.GetComponent<Feet>();
            if (incoming != null) SetActivator(incoming);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (listenToCollision)
        {
            Feet leaving = null;
            if (collision.collider != null)
                leaving = collision.collider.GetComponent<Feet>();
            if (leaving != null && leaving == activator) SetActivator(null);
        }
    }

    public void SetActivator(Feet activatorFeet)
    {
        if (activatorFeet != null)
        {
            if (isFirstLink)
            {
                if (activator == null)
                {
                    activator = activatorFeet;
                    currentState.Value = LinkState.Passed;
                    isCurrentAndActivated.Value = true;
                }
                else if (activator == activatorFeet)
                    isCurrentAndActivated.Value = true;
            }
            else
            {
                if (activator == activatorFeet)
                {
                    currentState.Value = LinkState.Passed;
                    isCurrentAndActivated.Value = true;
                }
            }
        }
        else
        {
            if (currentState == (int)LinkState.Passed)
                isCurrentAndActivated.Value = false;
        }
    }

    protected override void OnFarInLine()
    {
        base.OnFarInLine();
        listenToCollision = false;
        sr.color = Color.clear;
    }

    protected override void OnPassed()
    {
        base.OnPassed();
        listenToCollision = true;
        if (activator != null)
        {
            activator.IsOnGround.AddValueListener<bool>(OnActivatorTouchesGround, false);

            if (nextLink != null)
            {
                sr.color = isCurrentAndActivated ? Color.white : Color.gray;

                if (nextLink is JumpChainLink)
                {
                    JumpChainLink nextJumpChainLink = nextLink as JumpChainLink;
                    nextJumpChainLink.activator = activator;
                }
            }
        }
    }

    protected override void OnNextInLine()
    {
        base.OnNextInLine();
        listenToCollision = true;
        sr.color = Color.yellow;
    }

    protected override void OnChainCompleted()
    {
        base.OnChainCompleted();
        listenToCollision = false;
        sr.color = Color.blue;
        if (activator != null)
            activator.IsOnGround.RemoveValueListener<bool>(OnActivatorTouchesGround);
    }

    protected override void OnChainFailed()
    {
        base.OnChainFailed();
        listenToCollision = false;
        sr.color = Color.red;
        if (activator != null)
        {
            activator.IsOnGround.RemoveValueListener<bool>(OnActivatorTouchesGround);
            activator = null;
        }
    }

    protected override void OnNextLinkPassed()
    {
        base.OnNextLinkPassed();

        if (activator != null)
            activator.IsOnGround.RemoveValueListener<bool>(OnActivatorTouchesGround);
    }

    private void OnActivatorTouchesGround(bool onGround)
    {
        if (onGround == true && nextLink != null && isCurrentAndActivated == false)
            nextLink.currentState.Value = LinkState.ChainFailed;
    }

    private void OnCurrentAndActivated(bool currentAndActivated)
    {
        if (currentAndActivated)
        {
            if (currentState != (int)LinkState.ChainCompleted)
                sr.color = Color.yellow;            
        }
        else
        {
            if (currentState == (int)LinkState.Passed)
                sr.color = Color.white;
        }
    }
}
