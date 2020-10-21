using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class LowPolyPolygonColliderShape : LowPolyShape
{
    [SerializeField] private PolygonCollider2D col;

    public override Component Component
    {
        get
        {
            return col;
        }

        set
        {
            if (value is PolygonCollider2D)
                col = value as PolygonCollider2D;
            else
                col = null;
        }
    }

    public LowPolyPolygonColliderShape(PolygonCollider2D polygonCollider)
    {
        col = polygonCollider;

        localPositions = new List<Vector2>(col.points).ConvertAll<Vector3>(x => (Vector3)x);
    }

    public override void UpdateShape()
    {
        if (hasChanged)
        {
            col.points = localPositions.ConvertAll<Vector2>(pos => pos).ToArray();
            
            hasChanged = false;
        }
    }   

    public override string ComponentName()
    {
        return "Collider";
    }
}
