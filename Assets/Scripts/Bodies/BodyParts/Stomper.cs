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

    private void OnEnable()
    {
        if (gravity != null)
        {
            gravity.isFalling.AddValueListener<bool>(OnFall);
            OnFall((bool)gravity.isFalling.Value);
        }
    }

    private void OnDisable()
    {
        if (gravity != null)
            gravity.isFalling.RemoveValueListener<bool>(OnFall);
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
        while ((bool)gravity.isFalling.Value == true)
        {
            if (gravity.FallSpeed >= velocityThreshold)
                stompCollider.enabled = true;

            yield return new WaitForFixedUpdate();
        }
    }
}
