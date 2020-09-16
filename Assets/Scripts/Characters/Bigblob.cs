using System.Collections;
using UnityEngine;

public class Bigblob : MonoBehaviour
{
    public class OrderComparer : System.Collections.Generic.IComparer<Bigblob>
    {
        public int Compare(Bigblob x, Bigblob y)
        {
            if (x != null && y != null)
            {
                return x.blobOrder - y.blobOrder;
            }

            return 0;
        }
    }

    public int blobOrder;
    public BoolChangeEvent isAwake;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAwake.Value = true;
        TransferBodyColor(collision.attachedRigidbody.GetComponent<Body>());
    }

    private void TransferBodyColor(Body body)
    {
        if (body != null)
        {
            body.render.color = GetComponentInChildren<SpriteRenderer>().color;
            Light bodyLight = body.GetComponentInChildren<Light>();
            if (bodyLight != null)
                bodyLight.color = GetComponentInChildren<Light>().color;
        }
    }
}
