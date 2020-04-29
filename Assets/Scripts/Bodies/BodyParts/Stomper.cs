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
            gravity.IsFalling.AddListener(OnFall);
            OnFall(gravity.IsFalling.Value);
        }
    }

    private void OnDisable()
    {
        if (gravity != null)
            gravity.IsFalling.RemoveListener(OnFall);
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
        while (gravity.IsFalling.Value == true)
        {
            if (gravity.FallSpeed >= velocityThreshold)
                stompCollider.enabled = true;

            yield return new WaitForFixedUpdate();
        }
    }
}
