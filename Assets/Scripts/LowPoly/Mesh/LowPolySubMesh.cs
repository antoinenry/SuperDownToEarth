using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
[System.Serializable]
public class LowPolySubMesh : MonoBehaviour
{
    [HideInInspector] public Color meshColor = Color.white;
    [HideInInspector] public UnityEvent OnChange;

    public Vector3[] Contour { get => contour; private set { } }
    public Vector3 Center { get; private set; }

    [SerializeField] private Vector3[] contour;

    private void OnDrawGizmosSelected()
    {
        if (contour != null)
        {
            Gizmos.color = Color.blue;

            for (int i = 0, iend = contour.Length; i < iend; i++)
                Gizmos.DrawLine(contour[i] + transform.position, contour[(i + 1) % iend] + transform.position);
        }
    }

    private void Awake()
    {
        OnChange = new UnityEvent();

        LowPolyShape lps = LowPolyShape.New(this);
        lps.CopyPositionFromOtherShapeComponent();
    }

    public int GetPositionCount()
    {
        if (contour == null)
            return 0;
        else
            return contour.Length;
    }

    public void SetPositions(Vector3[] localPositions)
    {
        contour = localPositions;
        UpdateCenter();
        OnChange.Invoke();
    }

    public void SetPosition(int index, Vector3 localPosition)
    {
        contour[index] = localPosition;
        UpdateCenter();
        OnChange.Invoke();
    }

    private void UpdateCenter()
    {
        Center = Vector3.zero;

        if(contour != null)
        {
            foreach (Vector3 pos in contour)
                Center += pos;
            Center /= contour.Length;
        }
    }

    public Vector3[] GetTriangles()
    {
        List<Vector3> triangles = new List<Vector3>();
        for(int i = 0, iend = contour.Length; i < iend; i++)
        {
            triangles.Add(contour[i] + transform.position);
            triangles.Add(contour[(i + 1) % iend] + transform.position);
            triangles.Add(Center + transform.position);
        }

        return triangles.ToArray();
    }
}
