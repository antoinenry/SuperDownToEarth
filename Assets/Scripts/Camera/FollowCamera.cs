using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public bool followX = true;
    public bool followY = true;
    public float damping = 10f;

    private void Update()
    {
        Vector3 moveTo = new Vector3(followX ? target.position.x : transform.position.x, followY ? target.position.y : transform.position.y, transform.position.z);
        if (damping > 0f)
            transform.position = Vector3.Lerp(transform.position, moveTo, 1f / damping);
        else
            transform.position = moveTo;
    }
}
