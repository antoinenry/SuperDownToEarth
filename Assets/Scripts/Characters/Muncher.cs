using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Muncher : MonoBehaviour
{
    public List<string> eatableTags;
    [Min(0f)] public float munchDelay;
    [Min(0f)] public float captureDelay;
    [Min(0f)] public float chewDuration;
    [Min(0f)] public float spitDelay;
    public Transform spitPoint;
    public float spitForce;

    [HideInInspector] public UnityEvent OnMunch;
    public ValueChangeEvent<bool> IsFull;

    private bool isClosed;
    private List<GameObject> preys;
    private float munchTimer;
    private Collider2D trigger;

    private void Awake()
    {
        OnMunch = new UnityEvent();
        IsFull = new ValueChangeEvent<bool>();
        preys = new List<GameObject>();
        trigger = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isClosed == false && preys.Count > 0)
        {
            if (munchTimer > munchDelay)
                StartCoroutine(MunchCoroutine());
            else if (munchTimer >= 0f)
                munchTimer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        preys.Add(collision.gameObject);
        IsFull.Value = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isClosed) return;

        preys.Remove(collision.gameObject);
        if (preys.Count == 0)
        {
            IsFull.Value = false;
            munchTimer = 0f;
        }
    }

    private IEnumerator MunchCoroutine()
    {
        munchTimer = -1f;
        OnMunch.Invoke();

        yield return new WaitForSeconds(captureDelay);

        isClosed = true;
        trigger.enabled = false;

        if (preys.Count > 0)
        {
            foreach (GameObject caughtPrey in preys)
                Capture(caughtPrey);

            yield return new WaitForSeconds(chewDuration);

            for (int i = 0;  i < preys.Count; i++)
                Digest(preys[i]);
        }
        
        IsFull.Value = false;
        yield return new WaitForSeconds(spitDelay);

        foreach (GameObject caughtPrey in preys)
            SpitOut(caughtPrey);

        preys.Clear();

        isClosed = false;
        trigger.enabled = true;
        munchTimer = 0f;
    }

    private void Capture(GameObject prey)
    {
        prey.transform.SetPositionAndRotation(spitPoint.transform.position, transform.rotation);
        
        Body preyBody = prey.GetComponent<Body>();
        if (preyBody != null) preyBody.enabled = false;
    }

    private void Digest(GameObject prey)
    {
        if (prey == null) return;
        
        foreach (Body body in prey.GetComponentsInChildren<Body>(true))
        {
            if (eatableTags.Contains(body.tag))
                body.Kill();
        }
    }

    private void SpitOut(GameObject prey)
    {
        if (prey == null) return;

        Body preyBody = prey.GetComponent<Body>();
        if (preyBody != null)
        {
            preyBody.enabled = true;
            if (preyBody.AttachedRigidBody != null)  preyBody.AttachedRigidBody.AddForce (spitForce * spitPoint.up);
        }
    }
}
