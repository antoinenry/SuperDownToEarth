using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameObject : SaveSystem
{
    public Component[] componentStatesToSave;
       
    private void Start()
    {
        isLoading.AddValueListener<bool>(OnLoad);
        isSaving.AddValueListener<bool>(OnSave);
    }

    public int GetCurrentStates(out ComponentState[] currentStates)
    {
        if (componentStatesToSave == null)
        {
            currentStates = null;
            return 0;
        }
        else
        {
            int stateCount = componentStatesToSave.Length;
            currentStates = new ComponentState[stateCount];
            for(int i = 0; i < stateCount; i++)
                currentStates[i] = ComponentState.New(componentStatesToSave[i]);
            return stateCount;
        }
    }

    public virtual void OnSave(bool inProgress)
    {
        if (inProgress)
        {
            //savedTransform.position = transform.position;
            //savedTransform.rotation = transform.rotation.eulerAngles.z;
        }
    }

    public virtual void OnLoad(bool inProgress)
    {
        if (inProgress)
        {
            //transform.position = savedTransform.position;
            //transform.rotation = Quaternion.Euler(0f, 0f, savedTransform.rotation);
        }
    }

    
}
