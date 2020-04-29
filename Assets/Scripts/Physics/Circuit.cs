using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    public Vector2[] localPoints = new Vector2[0];
    [Min(0)] public int numCornerVertices;
    [Min(0f)] public float cornerRadius;
    public bool loop = true;

    [HideInInspector] [SerializeField]
    private Vector2[] trajectory;

    public int Length
    {
        get => trajectory == null ? localPoints.Length : trajectory.Length;
    }

    private void Awake()
    {
        UpdateTrajectory();
    }

    public void OnDrawGizmosSelected()
    {
        if (Length < 2) return;

        Gizmos.color = Color.magenta;
        for (int i = 0; i < Length - 1; i++)
            Gizmos.DrawLine(GetPoint(i), GetPoint(i + 1));

        if (loop) Gizmos.DrawLine(GetPoint(Length - 1), GetPoint(0));
    }

    public Vector3 GetPoint(int index)
    {
        if (trajectory == null || index >= Length)
            UpdateTrajectory();

        if (trajectory == null || index >= Length)
            return Vector3.one;

        return transform.position + (Vector3)trajectory[index];
    }

    public void UpdateTrajectory()
    {
        trajectory = new Vector2[localPoints.Length];
        localPoints.CopyTo(trajectory, 0);
        trajectory = ChaikinSmoothLine.Smoothen(trajectory, loop, cornerRadius, numCornerVertices);
    }
}