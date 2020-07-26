using UnityEngine;

public class Body : MonoBehaviour
{
    [Header("Body")]
    public Trigger destroyBody;
    public Trigger respawnBody;

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

    protected virtual void Awake()
    {
        parts = GetComponentsInChildren<BodyPart>(true);
    }

    protected virtual void Start()
    {
        destroyBody.AddTriggerListener(OnDestroyBody);
        respawnBody.AddTriggerListener(OnRespawnBody);
    }

    protected virtual void OnDestroy()
    {
        destroyBody.RemoveTriggerListener(OnDestroyBody);
        respawnBody.RemoveTriggerListener(OnRespawnBody);
    }

    protected virtual void OnDestroyBody()
    {
        foreach (BodyPart part in parts)
            part.enabled = false;
    }

    protected virtual void OnRespawnBody()
    {
        foreach (BodyPart part in parts)
            part.enabled = true;
    }
    
    /*
     * Tout ça devrait être géré ailleurs
     * 
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
