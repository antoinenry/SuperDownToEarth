using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Stomper : BodyPart
{
    public LocalGravity gravity;
    public float velocityThreshold;

    private Collider2D stompCollider;

    private void Awake()
    {
        stompCollider = GetComponent<Collider2D>();
    }    

    public override void OnEnable()
    {
        base.OnEnable();

        if (gravity != null)
        {
            gravity.IsFalling.AddListener<bool>(OnFall);
            OnFall(gravity.IsFalling.Get<bool>());
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (gravity != null)
            gravity.IsFalling.RemoveListener<bool>(OnFall);
    }

    private void OnFall(bool isFalling)
    {
        StopAllCoroutines();
        if (isFalling)
            StartCoroutine(FallCoroutine());
        else
            stompCollider.enabled = false;
    }

    private IEnumerator FallCoroutine()
    {
        while (gravity.IsFalling.Get<bool>() == true)
        {
            if (gravity.FallSpeed >= velocityThreshold)
                stompCollider.enabled = true;

            yield return new WaitForFixedUpdate();
        }
    }
}
