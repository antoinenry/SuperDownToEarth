using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LowPolyModel : MonoBehaviour
{  
    public LowPolyModel referenceModel;
    public int changeCount;

    private void Update()
    {
        if (referenceModel != null && changeCount != referenceModel.changeCount)
        {
            referenceModel.CopyModelTo(this);
        }
    }
    
    public bool ValuesEquals(LowPolyModel other)
    {
        Component[] thisComponents = this.GetComponentsInChildren<Component>(true);
        Component[] otherComponents = other.GetComponentsInChildren<Component>(true);

        int numComponents = thisComponents.Length;

        if (otherComponents.Length != numComponents)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < numComponents; i++)
            {
                if (LowPolyModelValues.Equality(thisComponents[i], otherComponents[i]) == false)
                    return false;
            }
        }

        return true;
    }

    public void CopyModelTo (LowPolyModel other)
    {
        Component[] theseComponents = this.GetComponentsInChildren<Component>(true);
        Component[] otherComponents = other.GetComponentsInChildren<Component>(true);

        int numLRs = theseComponents.Length;

        if (otherComponents.Length == numLRs)
        {
            for (int i = 0; i < numLRs; i++)
            {
                LowPolyModelValues.CopyFromTo(theseComponents[i], otherComponents[i]);
            }
        }
        else
        {
            Debug.Log("Numcomponents error");
        }

        other.changeCount = changeCount;
    }
}
