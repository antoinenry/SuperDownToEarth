using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : SaveComponent //, IValueChangeEventsComponent
{
    [Header("Body")]
    public ValueChangeEvent IsDead = ValueChangeEvent.New<bool>();

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

    public virtual int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { IsDead };
        return vces.Length;
    }

    public virtual int SetValueChangeEventsID()
    {
        IsDead.SetID("IsDead", this, 0);
        return 1;
    }

    public virtual void EnslaveValueChangeEvents(bool enslave)
    {
        IsDead.Enslave(enslave);
    }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        SaveSystem.InitSaveSystem();

        parts = GetComponentsInChildren<BodyPart>(true);
    }

    public virtual void Kill()
    {
        IsDead.Set(true);
        foreach (BodyPart part in parts)
            part.enabled = false;
    }

    public virtual void Respawn()
    {
        IsDead.Set(false);
        foreach (BodyPart part in parts)
            part.enabled = true;
    }
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
}
