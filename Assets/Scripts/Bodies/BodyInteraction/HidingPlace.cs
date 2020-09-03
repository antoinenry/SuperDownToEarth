using UnityEngine;
using System.Collections;

public class HidingPlace : MonoBehaviour
{
    public Transform pushOutPoint;
    public float pushOutForce = 0f;
    public float pushOutAngle = 0f;
    public bool jumpTriggersExit = true;
    public bool exitTriggersJump = true;
    public string triggerTag = "Player";
    public float triggerCoolDown = .5f;

    public Trigger pushOut;
    public BoolChangeEvent isHidingABody;

    protected Body hiddenBody;
    protected Jumper hiddenJumper;
    protected bool triggerIsCoolingDown;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggerIsCoolingDown == false && collision.collider.CompareTag(triggerTag) == true)
            OnBodyEnter(collision.collider.attachedRigidbody.GetComponent<PhysicalBody>(), collision.relativeVelocity.magnitude);
    }

    private  void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerIsCoolingDown == false && collision.CompareTag(triggerTag) == true)
            OnBodyEnter(collision.attachedRigidbody.GetComponent<PhysicalBody>(), collision.attachedRigidbody.velocity.magnitude);
    }

    protected virtual void OnBodyEnter(PhysicalBody body, float velocity)
    {
        Hide(body);
    }

    public void Hide(Body body)
    {
        if (isHidingABody == false && body != null)
        {
            body.Visible = false;
            body.transform.position = transform.position;

            if (body is PhysicalBody)
            {
                PhysicalBody physical = body as PhysicalBody;
                physical.Simulate = false;
                physical.Move = false;

                if (jumpTriggersExit || exitTriggersJump)
                {
                    hiddenJumper = body.GetComponent<Jumper>();
                    if (hiddenJumper != null && jumpTriggersExit)
                        hiddenJumper.tryJump.AddTriggerListener(OnPushOut);
                }
            }            

            isHidingABody.Value = true;
        }

        hiddenBody = body;
    }

    public void PushOut()
    {
        pushOut.Trigger();
    }

    protected virtual void OnPushOut()
    {
        StartCoroutine(PushOutCoroutine());
    }

    private IEnumerator PushOutCoroutine()
    {
        triggerIsCoolingDown = true;
        isHidingABody.Value = false;

        if (hiddenBody != null)
        {
            hiddenBody.Visible = true;

            Transform pushOutTransform = pushOutPoint == null ? transform : pushOutPoint;            
            hiddenBody.transform.rotation = pushOutTransform.rotation;
            hiddenBody.transform.position = pushOutTransform.position;
            if (hiddenBody is PhysicalBody)
            {
                PhysicalBody physical = hiddenBody as PhysicalBody;
                physical.Simulate = true;
                physical.Move = true;
                if (pushOutForce != 0f)
                    physical.AttachedRigidBody.AddForce(Quaternion.Euler(0f, 0f, pushOutAngle) * pushOutTransform.up * pushOutForce);
            }

            hiddenBody = null;
        }

        if (hiddenJumper != null)
        {
            if (jumpTriggersExit)
                hiddenJumper.tryJump.RemoveTriggerListener(OnPushOut);

            if (exitTriggersJump)
            {
                yield return new WaitForFixedUpdate();
                hiddenJumper.Jump(true);
            }

            hiddenJumper = null;
        }

        yield return new WaitForSeconds(triggerCoolDown);
        triggerIsCoolingDown = false;
    }
}
