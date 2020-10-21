using System.Collections.Generic;
using UnityEngine;

public abstract class SaveSystem : MonoBehaviour
{
    static public BoolChangeEvent isSaving;
    static public BoolChangeEvent isLoading;

    static protected List<ComponentState> saveStatesBuffer;

    private void Awake()
    {
        InitSaveSystem();
    }

    public static void InitSaveSystem()
    {
        if (isSaving == null) isSaving = new BoolChangeEvent();
        if (isLoading == null) isLoading = new BoolChangeEvent();
    }

    public static void Save()
    {
        saveStatesBuffer = new List<ComponentState>();

        isSaving.Value = true;
        isSaving.Value = false;
        
        saveStatesBuffer.Clear();
        saveStatesBuffer.Capacity = 0;
    }
}
