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

    private RendererColorModifier colorModifier;

    private void Awake()
    {
        colorModifier = GetComponentInChildren<RendererColorModifier>(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAwake.Value = true;
        RendererColorModifier blobColor = collision.attachedRigidbody.GetComponentInChildren<RendererColorModifier>();
        if (blobColor != null && colorModifier != null) blobColor.SetColor(colorModifier.currentColor);
    }
}
