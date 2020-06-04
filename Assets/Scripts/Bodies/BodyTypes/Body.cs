using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [Header("Body")]
    public BoolChangeEvent IsDead;

    public BodyPart[] parts { get; private set; }

    public virtual Rigidbody2D AttachedRigidBody
    {
        get => null;
        protected set { }
    }

    public virtual SpriteRenderer AttachedRenderer
    {
        get => null;
        protected set { }
    }

    private void Awake()
    {
        parts = GetComponentsInChildren<BodyPart>(true);
    }

    public virtual void Kill()
    {
        IsDead.Value = true;
        foreach (BodyPart part in parts)
            part.enabled = false;
    }

    public virtual void Respawn()
    {
        IsDead.Value = false;
        foreach (BodyPart part in parts)
            part.enabled = true;
    }
    
    /*
    public override void OnCheckPointSave(Transform checkPoint)
    {
        //Debug.Log("CheckPoint save " + name);

        if (IsDead.Get<bool>() == true)
        {
            Destroy(this.gameObject);
            return;
        }

        base.OnCheckPointSave(checkPoint);
    }

    public override void OnCheckPointLoad(Transform checkPoint)
    {
        StartCoroutine(LoadCheckPointCoroutine(checkPoint));
    }

    private IEnumerator LoadCheckPointCoroutine(Transform checkPoint)
    {
        Kill();
        yield return new WaitForFixedUpdate();
        base.OnCheckPointLoad(checkPoint);
        yield return new WaitForFixedUpdate();
        Respawn();
    }
    */
}
