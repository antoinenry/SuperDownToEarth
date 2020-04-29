using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CheckPointTrigger : SaveSystem
{
    public string checkPointTag = "CheckPoint";
    public Transform lastCheckpoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (lastCheckpoint != collision.transform && collision.CompareTag(checkPointTag))
        {
            SaveSystem.CheckPointSaveEvent.Invoke(collision.transform);
            lastCheckpoint = collision.transform;
        }
    }

    public void LoadCheckPoint()
    {
        SaveSystem.CheckPointLoadEvent.Invoke(lastCheckpoint);
    }
}
