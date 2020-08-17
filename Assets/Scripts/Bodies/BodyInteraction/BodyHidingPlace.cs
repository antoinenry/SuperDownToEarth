using UnityEngine;
using System.Collections;

public class BodyHidingPlace : MonoBehaviour
{
    public string hideTag = "Player";
    public float pushOutForce = 100f;
    public float pushOutCooldown = 1f;
    public Trigger pushOut;
    public BoolChangeEvent isHidingABody;

    private PhysicalBody hiddenBody;
    private Jumper hiddenJumper;
    private bool isCoolingDown;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCoolingDown == true || isHidingABody == true || collision.CompareTag(hideTag) == false) return;

        hiddenBody = collision.GetComponent<PhysicalBody>();
        if (hiddenBody != null)
        {
            HideBody(hiddenBody, true);
            hiddenJumper = hiddenBody.GetComponent<Jumper>();
            if (hiddenJumper != null)
                hiddenJumper.tryJump.AddTriggerListener(OnPushOut);
        }

    }

    private void OnPushOut()
    {
        StartCoroutine(PushOutCoroutine());
    }

    private void HideBody(PhysicalBody body, bool hiding)
    {
        isHidingABody.Value = hiding;
        body.Visible = !hiding;
        body.Simulate = !hiding;
        body.Move = !hiding;
    }

    private IEnumerator PushOutCoroutine()
    {
        if (hiddenBody != null)
        {
            HideBody(hiddenBody, false);
            hiddenBody.transform.rotation = transform.rotation;
            hiddenBody.transform.position = transform.position;
            //hiddenBody.AttachedRigidBody.AddForce(transform.up * pushOutForce);
            hiddenBody = null;
        }

        if (hiddenJumper != null)
        {
            hiddenJumper.tryJump.RemoveTriggerListener(OnPushOut);
            yield return new WaitForFixedUpdate();
            hiddenJumper.Jump(true);
            hiddenJumper = null;
        }

        isCoolingDown = true;
        yield return new WaitForSeconds(pushOutCooldown);
        isCoolingDown = false;
    }

    /*
    private void OnTriggerExit2D(Collider2D collision)
    {
        Body hideBody = collision.GetComponent<Body>();
        if (hideBody != null) hideBody.Visible = true;

        HitBox hideHitBox = collision.GetComponentInChildren<HitBox>();
        if (hideHitBox != null) hideHitBox.enabled = true;
    }
    */
}
