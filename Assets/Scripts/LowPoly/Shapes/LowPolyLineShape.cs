using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LowPolyLineShape : LowPolyShape
{
    [SerializeField] private LineRenderer lr;

    public override Component Component
    {
        get
        {
            return lr;
        }

        set
        {
            if (value is LineRenderer)
                lr = value as LineRenderer;
            else
                lr = null;
        }
    }

    public LowPolyLineShape(LineRenderer lineRenderer)
    {
        lr = lineRenderer;

        Vector3[] linePositions = new Vector3[lr.positionCount];
        lr.GetPositions(linePositions);
        localPositions = new List<Vector3>(linePositions);
    }

    public override void UpdateShape()
    {
        if(hasChanged)
        {
            lr.positionCount = PositionCount;
            lr.SetPositions(localPositions.ToArray());
            
            hasChanged = false;
        }
    }

    public override string ComponentName()
    {
        return "Line";
    }
}