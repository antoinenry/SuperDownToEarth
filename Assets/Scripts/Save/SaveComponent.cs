using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveComponent : SaveSystem
{
    [Header("Save Component")]
    public bool useCheckPointTransform;

    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private Vector3 savedScale;

    private void Start()
    {
        SaveSystem.CheckPointLoadEvent.AddListener(OnCheckPointLoad);
        SaveSystem.CheckPointSaveEvent.AddListener(OnCheckPointSave);
    }

    public virtual void OnCheckPointSave(Transform checkPoint)
    {
        //Debug.Log("Save " + name);

        if (useCheckPointTransform)
        {
            savedPosition = checkPoint.position;
            savedRotation = checkPoint.rotation;
            savedScale = checkPoint.localScale;

        }
        else
        {
            savedPosition = transform.position;
            savedRotation = transform.rotation;
            savedScale = transform.localScale;
        }
    }

    public virtual void OnCheckPointLoad(Transform checkPoint)
    {
        //Debug.Log("Load " + name);
        //Debug.Break();

        transform.position = savedPosition;
        transform.rotation = savedRotation;
        transform.localScale = savedScale;
    }
}
