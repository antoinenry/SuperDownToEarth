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
}
