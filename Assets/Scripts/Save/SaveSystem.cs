using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public abstract class SaveSystem : MonoBehaviour
{
    public class CheckPointEvent : UnityEvent<Transform> { }

    static public CheckPointEvent CheckPointSaveEvent;
    static public CheckPointEvent CheckPointLoadEvent;

    private void Awake()
    {
        SaveSystem.InitSaveSystem();
    }

    public static void InitSaveSystem()
    {
        if (CheckPointLoadEvent == null) CheckPointLoadEvent = new CheckPointEvent();
        if (CheckPointSaveEvent == null) CheckPointSaveEvent = new CheckPointEvent();
    }
}
