using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poscillator : MonoBehaviour
{
    public Vector2 amplitude;
    public Vector2 frequency;

    private Vector3 startPosition;
    private float startTime;

    private void Start()
    {
        startPosition = transform.localPosition;
        startTime = Time.fixedTime;
    }

    private void FixedUpdate()
    {
        float time = Time.fixedTime - startTime;

        Vector3 offPos = new Vector3(
            amplitude.x * Mathf.Sin(time * frequency.x * (2 * Mathf.PI)),
            amplitude.y * Mathf.Sin(time * frequency.y * (2 * Mathf.PI)),
            0f);

        transform.localPosition  = startPosition + offPos;
    }
}
