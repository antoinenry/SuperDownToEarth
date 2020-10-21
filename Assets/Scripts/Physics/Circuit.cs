using UnityEngine;
using System.Collections.Generic;

public class Circuit : MonoBehaviour
{        
    //[Range(0, 5)] public int numCornerVertices;
    //[Min(0f)] public float cornerRadius;
    public bool loop;

    [SerializeField]
    private Vector2[] controlPoints = new Vector2[0];

    public int Length => controlPoints != null ? controlPoints.Length : 0;

    public void OnDrawGizmosSelected()
    {
        Vector2 circuitPosition = transform.position;
        if (Length < 2) return;

        Gizmos.color = Color.magenta;
        for (int i = 0; i < Length - 1; i++)
            Gizmos.DrawLine(circuitPosition + controlPoints[i], circuitPosition + controlPoints[i+1]);
        if (loop) Gizmos.DrawLine(circuitPosition + controlPoints[Length - 1], circuitPosition + controlPoints[0]);
    }

    public Vector2? GetPosition(int index)
    {
        if (controlPoints == null || index < 0 || index >= Length) return null;
        else return (Vector2)transform.position + controlPoints[index];
    }    
}