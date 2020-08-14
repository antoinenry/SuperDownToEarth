using UnityEngine;

public class Body : MonoBehaviour
{
    [Header("Body")]
    public Trigger destroyBody;
    public Trigger respawnBody;

    public bool IsAlive { get; private set; }
    public BodyPart[] Parts { get; private set; }

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
        Parts = GetComponentsInChildren<BodyPart>(true);
        IsAlive = true;
    }

    protected virtual void Start()
    {
        destroyBody.AddTriggerListener(OnDestroyBody);
        respawnBody.AddTriggerListener(OnRespawnBody);
    }

    protected virtual void OnDestroy()
    {
        destroyBody.RemoveAllListeners();
        respawnBody.RemoveAllListeners();
    }

    protected virtual void OnDestroyBody()
    {
        IsAlive = false;
        foreach (BodyPart part in Parts)
            part.enabled = false;
    }

    protected virtual void OnRespawnBody()
    {
        IsAlive = true;
        foreach (BodyPart part in Parts)
            part.enabled = true;
    }

    public void Kill()
    {
        destroyBody.Trigger();
    }

    public void Respawn()
    {
        respawnBody.Trigger();
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
