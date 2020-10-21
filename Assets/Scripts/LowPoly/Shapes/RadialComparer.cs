using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialComparer : IComparer<Vector3>
{
    public Vector2 center;
    public Vector2 startDirection = Vector2.up;

    public int Compare(Vector3 a, Vector3 b)
    {
        if (startDirection == Vector2.zero)
            return 0;

        float angleDiff = Vector2.SignedAngle(startDirection, (Vector2)a - center) - Vector2.SignedAngle(startDirection, (Vector2)b - center);

        if (angleDiff == 0f)
            return 0;
        else
            return (angleDiff < 0f) ? 1 : -1;   
    }
}