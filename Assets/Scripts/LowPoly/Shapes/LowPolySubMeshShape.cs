using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LowPolySubMeshShape : LowPolyShape
{
    [SerializeField] private LowPolySubMesh lpsm;

    public override Component Component
    {
        get
        {
            return lpsm;
        }

        set
        {
            if (value is LowPolySubMesh)
                lpsm = value as LowPolySubMesh;
            else
                lpsm = null;
        }
    }

    public LowPolySubMeshShape(LowPolySubMesh lowPolySubMesh)
    {
        lpsm = lowPolySubMesh;        

        if (lpsm.Contour != null)
            localPositions = new List<Vector3>(lpsm.Contour);
        else
            localPositions = new List<Vector3>();

        UpdateShape();
    }

    public override void UpdateShape()
    {
        if (hasChanged)
        {
            lpsm.SetPositions(localPositions.ToArray());

            hasChanged = false;
        }
    }

    public override string ComponentName()
    {
        return "SubMesh";
    }
}
