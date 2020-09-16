using UnityEngine;
using System.Collections.Generic;

public class BlobList : MonoBehaviour
{
    private SpriteImageSync template;
    private List<Bigblob> blobs;

    private void Awake()
    {
        template = GetComponentInChildren<SpriteImageSync>(true);
        if (template == null) return;

        blobs = new List<Bigblob>(FindObjectsOfType<Bigblob>(true));

        if (blobs.Count == 0)
        {
            template.gameObject.SetActive(false);
        }
        else
        {
            blobs.Sort(new Bigblob.OrderComparer());

            SetBlobInterface(template, blobs[0]);
            for(int i = 1; i < blobs.Count; i++)
            {
                SpriteImageSync newSync = Instantiate(template, transform);
                SetBlobInterface(newSync, blobs[i]);
            }
        }
    }

    private void OnEnable()
    {
        SpriteImageSync[] syncs = GetComponentsInChildren<SpriteImageSync>(true);
        for (int i = 0; i < blobs.Count; i++)
        {
            if (i > syncs.Length) return;
            syncs[i].syncColor = blobs[i].isAwake;
        }
    }

    private void SetBlobInterface(SpriteImageSync sync, Bigblob bb)
    {
        sync.syncedSprite = bb.GetComponentInChildren<SpriteRenderer>();
        sync.syncColor = bb.isAwake;
    }
}
