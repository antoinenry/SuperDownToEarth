using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : BodyPart
{
    public LayerMask blockingLayers;

    private List<GameObject> inTrigger;

    private void OnDrawGizmosSelected()
    {
        if (inTrigger != null)
        {
            foreach (GameObject go in inTrigger)
            {
                if (IsViewBlocked(go) == false)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.gray;

                Gizmos.DrawLine(transform.position, go.transform.position);
            }
        }
    }

    private void Awake()
    {
        inTrigger = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inTrigger.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inTrigger.Remove(collision.gameObject);
    }

    public GameObject DetectedObject()
    {
        foreach (GameObject go in inTrigger)
        {
            if (IsViewBlocked(go) == false)
                return go;
        }

        return null;
    }

    public GameObject[] DetectedObjects()
    {
        List<GameObject> detected = new List<GameObject>();

        foreach(GameObject go in inTrigger)
        {
            if (IsViewBlocked(go) == false)
                detected.Add(go);
        }

        return detected.ToArray();
    }

    private bool IsViewBlocked(GameObject target)
    {
        Vector2 thisToObject = target.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, thisToObject, thisToObject.magnitude, blockingLayers);

        return (hit.collider != null && hit.collider.gameObject != target);
    }
}
