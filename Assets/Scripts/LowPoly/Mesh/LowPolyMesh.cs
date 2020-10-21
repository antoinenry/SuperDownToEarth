using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class LowPolyMesh : MonoBehaviour
{
    private MeshFilter mf;

    [SerializeField]
    private LowPolySubMesh[] subMeshes;

    private bool meshChanged;

    private Mesh CurrentMesh
    {
        get => Application.isPlaying ? mf.mesh : mf.sharedMesh;
        
        set
        {
            if (Application.isPlaying)
                mf.mesh = value;
            else
                mf.sharedMesh = value;
        }
    }

    public int SubMeshCount
    {
        get
        {
            if (subMeshes != null)
                return subMeshes.Length;
            else
                return 0;
        }

        private set { }
    }
    
    private void Awake()
    {
        mf = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        if (subMeshes == null)
            SetSubMeshesFromChildren();

        RemoveListeners();
        AddListeneners();
    }

    private void Update()
    {
        if (meshChanged)
        {
            UpdateMesh();
            meshChanged = false;
        }
    }

    public void CallMeshUpdate()
    {
        meshChanged = true;
        if (Application.isPlaying == false) UpdateMesh();
    }

    public void GetSubMeshes(LowPolySubMesh[] subs)
    {
        if (subMeshes != null)
            subMeshes.CopyTo(subs, 0);
        else
            subs = null;
    }

    public void SetSubMeshesFromChildren()
    {
        SetSubMeshes(GetComponentsInChildren<LowPolySubMesh>(true));
    }

    public void SetSubMeshes(LowPolySubMesh[] subs)
    {
        RemoveListeners();
        subMeshes = subs;
        AddListeneners();
    }

    public bool CompareSubMeshes(LowPolySubMesh[] subs)
    {
        if (subs == null)
            return subMeshes == null;

        if (subMeshes == null)
            return false;

        return Enumerable.SequenceEqual<LowPolySubMesh>(subs, subMeshes);
    }

    private void UpdateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        foreach (LowPolySubMesh sub in subMeshes)
        {
            if (sub != null)
            {
                List<Vector3> localPositions = new List<Vector3>(sub.GetTriangles()).ConvertAll<Vector3>(x => x - transform.position);
                foreach(Vector3 pos in localPositions)
                {
                    vertices.Add(pos);
                    colors.Add(sub.meshColor);
                }
            }                
        }

        List<int> triangles = new List<int>();
        for (int i = 0, iend = vertices.Count; i < iend; i++)
        {
            if (subMeshes != null)
                triangles.Add(i);
        }

        SetMesh(vertices.ToArray(), triangles.ToArray(), colors.ToArray());
    }

    private void SetMesh(Vector3[] vs, int[] ts, Color[] cs)
    {
        if (mf == null) return;

        if (CurrentMesh == null)
        {
            CurrentMesh = new Mesh
            {
                triangles = ts,
                vertices = vs,
                colors = cs
            };

            CurrentMesh.name = name + "Mesh";
        }
        else
        {
            if (vs.Length == 0) CurrentMesh.triangles = ts;

            CurrentMesh.vertices = vs;
            CurrentMesh.triangles = ts;
            CurrentMesh.colors = cs;
            CurrentMesh.uv = new List<Vector3>(vs).ConvertAll<Vector2>(v => v).ToArray();
        }

        CurrentMesh.RecalculateNormals();
        //CurrentMesh.RecalculateBounds();
    }

    private void AddListeneners()
    {
        if (subMeshes == null) return;

        foreach(LowPolySubMesh sub in subMeshes)
        {

            if (sub != null)
            {
                sub.OnChange.AddListener(CallMeshUpdate);
            }
        }
    }

    private void RemoveListeners()
    {
        if (subMeshes == null) return;

        foreach (LowPolySubMesh sub in subMeshes)
        {
            if (sub != null)
                sub.OnChange.RemoveListener(CallMeshUpdate);
        }
    }

    public void RemoveListeners(LowPolySubMesh[] subs)
    {
        if (subs == null) return;

        foreach (LowPolySubMesh sub in subs)
        {
            if (sub != null)
                sub.OnChange.RemoveListener(CallMeshUpdate);
        }
    }
}
