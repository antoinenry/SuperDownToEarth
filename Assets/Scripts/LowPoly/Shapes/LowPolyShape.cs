using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LowPolyShape
{
    protected List<Vector3> localPositions;
    protected bool hasChanged;

    public abstract Component Component { get; set; }

    public abstract void UpdateShape();
    public abstract string ComponentName();

    #region Component handling
    public static LowPolyShape New(Component component)
    {
        if (component != null)
        {
            if (component is LineRenderer)
                return new LowPolyLineShape(component as LineRenderer);

            if (component is PolygonCollider2D)
                return new LowPolyPolygonColliderShape(component as PolygonCollider2D);

            if (component is LowPolySubMesh)
                return new LowPolySubMeshShape(component as LowPolySubMesh);
        }

        return null;
    }

    public static Component[] GetShapeComponentsInGameObjects(GameObject[] gos, bool withChildren)
    {
        if (gos == null || gos.Length == 0) return null;

        List<Component> components = new List<Component>();
        foreach (GameObject go in gos)
        {
            foreach (LineRenderer lr in go.GetComponentsInChildren<LineRenderer>(true))
            {
                if (components.Contains(lr) == false)
                    components.Add(lr);
            }

            foreach (PolygonCollider2D pc in go.GetComponentsInChildren<PolygonCollider2D>(true))
            {
                if (components.Contains(pc) == false)
                    components.Add(pc);
            }

            foreach (LowPolySubMesh sm in go.GetComponentsInChildren<LowPolySubMesh>(true))
            {
                if (components.Contains(sm) == false)
                    components.Add(sm);
            }
        }

        return components.ToArray();
    }

    public static Component[] GetShapeComponentsInGameObject(GameObject go, bool withChildren)
    {
        return GetShapeComponentsInGameObjects(new GameObject[] { go }, withChildren);
    }
    #endregion

    #region Positions handling
    public void CallShapeUpdate()
    {
        hasChanged = true;
        if (Application.isPlaying == false) UpdateShape();
    }

    public int PositionCount
    {
        get => localPositions.Count;
        private set { }
    }

    public Vector3 GetPosition(int index)
    {
        return localPositions[index] + Component.transform.position;
    }

    public Vector3[] GetPositions()
    {
        Vector3[] worldPositions = localPositions.ConvertAll<Vector3>(pos => pos + Component.transform.position).ToArray();
        return worldPositions;
    }

    public void SetPosition(int index, Vector3 pos)
    {
        localPositions[index] = pos - Component.transform.position;
        CallShapeUpdate();
    }

    public void SetPositions(Vector3[] newPositions)
    {
        localPositions = new List<Vector3>(newPositions).ConvertAll<Vector3>(pos => pos - Component.transform.position);
        CallShapeUpdate();
    }

    public void InsertPosition(int atIndex, Vector3 pos)
    {
        localPositions.Insert(atIndex, pos - Component.transform.position);
        CallShapeUpdate();
    }

    public void InsertPosition(Vector3 pos1, Vector3 insertPos, Vector3 pos2)
    {
        if (PositionCount > 0)
        {
            Vector3 thisPos;
            Vector3 nextPos = GetPosition(0);

            for (int i = 0; i < PositionCount; i++)
            {
                thisPos = nextPos;
                nextPos = GetPosition((i + 1) % PositionCount);

                if ((thisPos == pos1 && nextPos == pos2) || (thisPos == pos2 && nextPos == pos1))
                {
                    InsertPosition((i + 1) % PositionCount, insertPos);
                    break;
                }
            }
        }
    }

    public void DeletePosition(Vector3 position)
    {
        localPositions.Remove(position - Component.transform.position);
        CallShapeUpdate();
    }

    public bool CopyPositionFromOtherShapeComponent()
    {
        if (Component == null)
            return false;

        Component[] otherComponents = GetShapeComponentsInGameObjects(new GameObject[] { Component.gameObject }, false);
        if (otherComponents == null) return false;

        foreach(Component other in otherComponents)
        {
            if (CopyPositionFromOtherShapeComponent(other) == true)
                return true;
        }

        return false;
    }

    public bool CopyPositionFromOtherShapeComponent(Component other)
    {
        if (Component == null || other == null)
            return false;

        if (other != Component)
        {
            LowPolyShape otherShape = LowPolyShape.New(other);
            localPositions = new List<Vector3>(otherShape.localPositions);
            CallShapeUpdate();
            return true;
        }

        return false;
    }

    public Vector3 LocalCenter
    {
        get
        {
            Vector3 center = Vector3.zero;

            if (localPositions.Count != 0)
            {
                foreach (Vector3 pos in localPositions)
                    center += pos;
                center /= localPositions.Count;
            }

            return center;
        }

        private set { }
    }

    public void SortPositions()
    {
        RadialComparer comparer = new RadialComparer
        {
            center = this.LocalCenter
        };
        
        localPositions.Sort(comparer);
    }

    public void DeleteNullSegments()
    {
        List<Vector3> newVertexPositions = new List<Vector3>();
        int numPositions = localPositions.Count;

        for (int i = 0; i < numPositions; i++)
        {
            if (localPositions[i] != localPositions[(i + 1) % numPositions])
                newVertexPositions.Add(localPositions[i]);
        }

        if (numPositions != newVertexPositions.Count)
            localPositions = newVertexPositions;
    }
    #endregion
}
