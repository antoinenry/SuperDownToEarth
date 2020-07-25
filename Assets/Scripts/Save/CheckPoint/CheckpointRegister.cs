using System.Collections;
using UnityEngine;

public class CheckpointRegister : MonoBehaviour
{
    public string checkpointTag = "Checkpoint";
    public Transform lastCheckpoint;

    public Trigger loadLastCheckpoint;
    [HideInInspector] public Trigger onCheckPointLoaded;

    private void Start()
    {
        if (loadLastCheckpoint == null) loadLastCheckpoint = new Trigger();
        loadLastCheckpoint.AddTriggerListener(OnLoadLastCheckpoint);
    }

    private void OnDestroy()
    {
        loadLastCheckpoint.RemoveTriggerListener(OnLoadLastCheckpoint);
    }

    public void LoadCheckpoint(Transform checkpointTransform)
    {
        if (lastCheckpoint == null) return;

        transform.position = lastCheckpoint.position;
        transform.rotation = lastCheckpoint.rotation;
        StartCoroutine(StopRigidbodiesCoroutine());        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(checkpointTag))
            RegisterCheckpoint(collision.transform);
    }

    private void RegisterCheckpoint(Transform checkpointTransform)
    {
        lastCheckpoint = checkpointTransform;
    }

    private void OnLoadLastCheckpoint()
    {
        LoadCheckpoint(lastCheckpoint);
    }

    private IEnumerator StopRigidbodiesCoroutine()
    {
        yield return new WaitForFixedUpdate();

        Rigidbody2D[] rigidbodies = GetComponentsInChildren<Rigidbody2D>(true);
        foreach (Rigidbody2D rb in rigidbodies)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        onCheckPointLoaded.Trigger();
    }
}
