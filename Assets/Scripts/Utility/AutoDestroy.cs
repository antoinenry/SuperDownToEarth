using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float destroyAfterSeconds = -1f;

    void Start()
    {
        if (destroyAfterSeconds >= 0)
            Destroy(gameObject, destroyAfterSeconds);
    }
}
