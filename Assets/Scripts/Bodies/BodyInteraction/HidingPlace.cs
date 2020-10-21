using UnityEngine;
using System.Collections;

public class HidingPlace : MonoBehaviour
{
    public Vector2 pushOutPoint;
    public Vector2 pushOutVelocity;
    public string triggerTag = "Player";
    public float triggerCoolDown = .5f;

    public Trigger pushOut;
    public BoolChangeEvent isHidingABody;

    protected Hider hider;
    protected bool triggerIsCoolingDown;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + transform.rotation * pushOutPoint, transform.position + transform.rotation * (pushOutPoint + pushOutVelocity.normalized));
    }

    private void OnEnable()
    {
        isHidingABody.AddValueListener<bool>(OnHiding);
        pushOut.AddTriggerListener(OnPushOut);
    }

    private void OnDisable()
    {
        isHidingABody.RemoveValueListener<bool>(OnHiding);
        pushOut.RemoveTriggerListener(OnPushOut);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggerIsCoolingDown == false && collision.collider.CompareTag(triggerTag) == true)
            Hide(collision.collider.attachedRigidbody.GetComponent<Hider>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerIsCoolingDown == false && collision.CompareTag(triggerTag) == true)
            Hide(collision.attachedRigidbody.GetComponent<Hider>());
    }

    public virtual void Hide(Hider target)
    {
        if (target == null) return;

        if (isHidingABody == false)
        {
            hider = target;
            hider.Hide();
            hider.transform.position = new Vector3(transform.position.x, transform.position.y, target.transform.position.z);
            hider.transform.rotation = transform.rotation;
            hider.stopHiding.AddTriggerListener(PushOut);

            isHidingABody.Value = true;
        }
    }

    public void PushOut()
    {
        pushOut.Trigger();
    }

    protected virtual void OnHiding(bool isHiding)
    {
        if (isHiding == false)
            PushOut();
    }

    protected virtual void OnPushOut()
    {
        StartCoroutine(PushOutCoroutine());
    }

    private IEnumerator PushOutCoroutine()
    {
        if (hider != null)
        {
            hider.stopHiding.RemoveTriggerListener(PushOut);
            hider.transform.position = new Vector3(transform.position.x, transform.position.y, hider.transform.position.z) + transform.rotation * Vector2.Scale(pushOutPoint, transform.lossyScale);
            hider.transform.rotation = transform.rotation;
            hider.StopHiding(transform.rotation * Vector2.Scale(pushOutVelocity, transform.lossyScale));
            hider = null;
        }

        triggerIsCoolingDown = true;
        isHidingABody.Value = false;

        yield return new WaitForSeconds(triggerCoolDown);
        triggerIsCoolingDown = false;
    }
}
