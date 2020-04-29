using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode]
public class LowPolyShapeHandler : MonoBehaviour
{
    [SerializeField] private LowPolyShape lps;
    [SerializeField] private Component lpsComponent;
    [SerializeField] private List<LowPolyPointHandle> pointHandles;
    private List<LowPolySegmentHandle> segmentHandles;

    public Component Shape
    {
        get
        {
            if (lps != null)
                return lps.Component;
            else
                return null;
        }

        set
        {
            if (lps != null)
            {
                if (lps.Component != value)
                    RemoveAllHandleListenners();
                else
                    return;
            }

            lps = LowPolyShape.New(value);
            lpsComponent = value;
        }
    }

    private void OnDrawGizmos()
    {
        if (Shape != null && Shape is Collider2D)
        {
            int posCount = lps.PositionCount;

            Gizmos.color = Color.green;
            for (int i = 0; i < posCount - 1; i++)
                Gizmos.DrawLine(lps.GetPosition(i), lps.GetPosition(i + 1));
            Gizmos.DrawLine(lps.GetPosition(posCount - 1), lps.GetPosition(0));
        }
    }

    #region Creation/Destruction
    public static LowPolyShapeHandler Instantiate(Component shapeComponent)
    {
        LowPolyShape shape = LowPolyShape.New(shapeComponent);
        if (shape == null) return null;

        LowPolyShapeHandler newHandler = new GameObject().AddComponent<LowPolyShapeHandler>();
        newHandler.lps = shape;
        newHandler.lpsComponent = shapeComponent;
        newHandler.name = shapeComponent.gameObject.name + "_" + shape.ComponentName() + "Handler";

        newHandler.UpdatePointHandles();
        newHandler.UpdateSegmentHandles();

        return newHandler;
    }

    private void OnDestroy()
    {
        RemoveAllHandleListenners();
    }

    public void DestroyTemporaryHandles()
    {
        RemoveTemporaryHandleListenners();
        UpdatePositions();
    }

    public bool IsPermanent()
    {
        return (pointHandles != null && pointHandles.FindIndex(h => h != null && h.permanent) != -1);
    }

    private bool RecoverShapeComponent()
    {
        if (Shape != null)
            return true;
        else
        {
            if (lpsComponent != null)
            {
                Shape = lpsComponent;
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Updates
    private void Awake()
    {
        RecoverShapeComponent();
    }

    private void Start()
    {
        if (pointHandles != null)
        {
            foreach (LowPolyPointHandle handle in pointHandles)
                AddHandleListenners(handle);
        }
        else
            DestroyImmediate(gameObject);
    }

    private void Update()
    {
        if (pointHandles == null)
        {
            UpdatePointHandles();

            if (pointHandles == null || pointHandles.Count == 0)
            {
                DestroyImmediate(gameObject);
                return;
            }
        }

        if (lps != null)
        {
            lps.UpdateShape();

            if (pointHandles.Count != lps.PositionCount)
            {
                UpdatePointHandles();
                UpdateSegmentHandles();
            }
        }
    }

    public void UpdatePointHandles()
    {
        CleanUpPointHandles();

        int posCount = lps.PositionCount;
        List<LowPolyPointHandle> obsoleteHandles = (pointHandles == null) ? null : new List<LowPolyPointHandle>(pointHandles);
        List<LowPolyPointHandle> newHandles = new List<LowPolyPointHandle>(new LowPolyPointHandle[posCount]);
        List<LowPolyPointHandle> allHandles = new List<LowPolyPointHandle>(GameObject.FindObjectsOfType<LowPolyPointHandle>());

        for (int i = 0; i < posCount; i++)
        {
            Vector3 wPos = lps.GetPosition(i);

            LowPolyPointHandle handle = null;

            if (pointHandles != null)
                handle = pointHandles.Find(h => h.transform.position == wPos);

            if (handle != null)
                obsoleteHandles.Remove(handle);
            else
            {
                handle = allHandles.Find(h => h.transform.position == wPos);
                if (handle == null)
                {
                    handle = LowPolyPointHandle.InstantiateHandle(wPos, "PointHandle_" + allHandles.Count);
                    allHandles.Add(handle);
                }
                AddHandleListenners(handle);
            }

            newHandles[i] = handle;
        }

        if (obsoleteHandles != null)
        {
            foreach (LowPolyPointHandle handle in obsoleteHandles)
                RemoveHandleListenners(handle);
        }

        pointHandles = newHandles;
    }

    public void UpdatePositions()
    {
        if (pointHandles == null || Shape == null || pointHandles.Count != lps.PositionCount)
            return;

        CleanUpPositions();

        for (int i = 0, iend = lps.PositionCount; i < iend; i++)
        {
            if (pointHandles[i] != null)
                lps.SetPosition(i, pointHandles[i].transform.position);
        }
    }

    private void UpdatePosition(Vector3 position)
    {
        CleanUpPositions();

        int posCount = lps.PositionCount;
        if (pointHandles.Count != posCount)
            return;

        for (int i = 0; i < posCount; i++)
        {
            if (lps.GetPosition(i) == position)
                lps.SetPosition(i, pointHandles[i].transform.position);
        }
    }

    private void UpdateSegmentHandles()
    {
        CleanUpSegmentHandles();

        int posCount = lps.PositionCount;
        if (posCount == 0) return;

        List<LowPolySegmentHandle> obsoleteHandles = (segmentHandles == null) ? null : new List<LowPolySegmentHandle>(segmentHandles);
        List<LowPolySegmentHandle> newHandles = new List<LowPolySegmentHandle>(new LowPolySegmentHandle[posCount]);
        List<LowPolySegmentHandle> allHandles = new List<LowPolySegmentHandle>(GameObject.FindObjectsOfType<LowPolySegmentHandle>());

        Vector3 wPosA;
        Vector3 wPosB = lps.GetPosition(0);
        for (int i = 0; i < posCount; i++)
        {
            wPosA = wPosB;
            wPosB = lps.GetPosition((i + 1) % posCount);
            Vector3 wPos = (wPosA + wPosB) / 2f;

            LowPolySegmentHandle handle = null;

            if (segmentHandles != null)
                handle = segmentHandles.Find(h => h.transform.position == wPos);

            if (handle != null)
                obsoleteHandles.Remove(handle);
            else
            {
                handle = allHandles.Find(h => h.transform.position == wPos);
                if (handle == null)
                {
                    handle = LowPolySegmentHandle.InstantiateHandle(pointHandles[i], pointHandles[(i + 1) % posCount], "SegmentHandle_" + allHandles.Count);
                    allHandles.Add(handle);
                }

                AddHandleListenners(handle);
            }

            newHandles[i] = handle;
            handle.UpdatePosition();
        }

        if (obsoleteHandles != null)
        {
            foreach (LowPolySegmentHandle handle in obsoleteHandles)
                RemoveHandleListenners(handle);
        }

        segmentHandles = newHandles;
    }
    #endregion

    #region OnHandleEvents
    private void OnMoveHandle(LowPolyPointHandle handle)
    {
        if (handle != null)
            UpdatePosition(handle.LastPosition);
    }

    private void OnDestroyHandle(LowPolyPointHandle handle)
    {
        if (handle != null)
            lps.DeletePosition(handle.transform.position);
    }

    public void OnFusionHandle(LowPolyPointHandle replaced)
    {
        int replacedIndex = pointHandles.IndexOf(replaced);
        if (replacedIndex != -1 && replaced.CloseHandle != null)
        {
            pointHandles[replacedIndex] = replaced.CloseHandle;
            UpdatePositions();
            UpdateSegmentHandles();
        }
    }

    public void OnMoveHandle(LowPolySegmentHandle handle)
    {
        if (handle != null)
        {
            lps.InsertPosition(handle.PtA.transform.position, handle.transform.position, handle.PtB.transform.position);
        }
    }
    #endregion

    #region HandlesListenners
    public void AddHandleListenners(LowPolyPointHandle handle)
    {
        if (handle != null)
        {
            handle.AddMoveAction(OnMoveHandle);
            handle.AddDestroyAction(OnDestroyHandle);
            handle.AddFusionAction(OnFusionHandle);
        }
    }

    public void RemoveHandleListenners(LowPolyPointHandle handle)
    {
        if (handle != null)
        {
            handle.RemoveMoveAction(OnMoveHandle);
            handle.RemoveDestroyAction(OnDestroyHandle);
            handle.RemoveFusionAction(OnFusionHandle);
        }
    }

    public void AddHandleListenners(LowPolySegmentHandle handle)
    {
        if (handle != null)
        {
            handle.AddMoveAction(OnMoveHandle);
        }
    }

    public void RemoveHandleListenners(LowPolySegmentHandle handle)
    {
        if (handle != null)
        {
            handle.RemoveMoveAction(OnMoveHandle);
        }
    }

    private void RemoveAllHandleListenners()
    {
        if (pointHandles != null)
        {
            foreach (LowPolyPointHandle handle in pointHandles)
                RemoveHandleListenners(handle);
            pointHandles = null;
        }

        if (segmentHandles != null)
        {
            foreach (LowPolySegmentHandle handle in segmentHandles)
                RemoveHandleListenners(handle);
            segmentHandles = null;
        }
    }

    private void RemoveTemporaryHandleListenners()
    {
        if (pointHandles != null)
        {
            foreach (LowPolyPointHandle handle in pointHandles)
            {
                if(handle != null && handle.permanent == false)
                    RemoveHandleListenners(handle);
            }
        }

        if (segmentHandles != null)
        {
            foreach (LowPolySegmentHandle handle in segmentHandles)
                RemoveHandleListenners(handle);
            segmentHandles = null;
        }
    }
    #endregion

    #region CleanUp
    private void CleanUpPointHandles()
    {
        if (pointHandles != null)
        {
            int handleCount = pointHandles.Count;
            List<LowPolyPointHandle> newHandles = new List<LowPolyPointHandle>();
            for (int i = 0; i < handleCount; i++)
            {
                if (pointHandles[i] != null && pointHandles[i].gameObject != null && pointHandles[i] != pointHandles[(i + 1) % handleCount])
                    newHandles.Add(pointHandles[i]);
            }

            if (newHandles.Count == 0)
                pointHandles = null;
            else if (newHandles.Count != pointHandles.Count)
                pointHandles = newHandles;
        }
    }

    private void CleanUpSegmentHandles()
    {
        if (segmentHandles != null)
        {
            int handleCount = segmentHandles.Count;
            List<LowPolySegmentHandle> newHandles = new List<LowPolySegmentHandle>();
            for (int i = 0; i < handleCount; i++)
            {
                if (segmentHandles[i] != null && segmentHandles[i].gameObject != null)
                    newHandles.Add(segmentHandles[i]);
            }

            if (newHandles.Count == 0)
                segmentHandles = null;
            else if (newHandles.Count != segmentHandles.Count)
                segmentHandles = newHandles;
        }
    }

    private void CleanUpPositions()
    {
        int posCount = lps.PositionCount;
        lps.DeleteNullSegments();

        if (posCount != lps.PositionCount)
            UpdatePointHandles();

        lps.UpdateShape();
    }
    #endregion
}
