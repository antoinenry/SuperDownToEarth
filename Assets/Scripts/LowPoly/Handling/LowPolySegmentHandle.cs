using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class LowPolySegmentHandle : MonoBehaviour
{
    public class HandleEvent : UnityEvent<LowPolySegmentHandle> { }

    private HandleEvent OnMoved;
    private List<UnityAction<LowPolySegmentHandle>> moveActions;

    public float closeDistance = 1f;
    private bool drag;
    
    public LowPolyPointHandle PtA { get; private set; }
    public LowPolyPointHandle PtB { get; private set; }

    public static LowPolySegmentHandle InstantiateHandle(LowPolyPointHandle A, LowPolyPointHandle B, string handleName = "segmentHandle")
    {
        if (A == null || B == null) return null;

        LowPolySegmentHandle newHandle = new GameObject().AddComponent<LowPolySegmentHandle>();
        newHandle.name = handleName;
        newHandle.SetPoints(A, B);        

        return newHandle;
    }

    private void OnDrawGizmos()
    {
        if(drag && IsOff())
        {
            Gizmos.DrawIcon(transform.position, "LowPolyEditor/Handle1");
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, PtA.transform.position);
            Gizmos.DrawLine(transform.position, PtB.transform.position);
        }
        else
            Gizmos.DrawIcon(transform.position, "LowPolyEditor/Handle3");
    }

    private void Awake()
    {
        if(OnMoved == null) OnMoved = new HandleEvent();
        if (moveActions == null) moveActions = new List<UnityAction<LowPolySegmentHandle>>();
    }

    private void OnDestroy()
    {
        RemoveAllListeners();
    }

    private void Update()
    {
        if (PtA == null || PtB == null || moveActions.Count == 0)
            DestroyImmediate(gameObject);
    }
    
    public bool Drag
    {
        get { return drag; }
        set
        {
            drag = value;
            if (drag == false && IsOff())
            {
                OnMoved.Invoke(this);
                DestroyImmediate(this.gameObject);
            }
        }
    }

    private bool IsOff()
    {
        return Vector3.Distance(transform.position, GetMiddlePosition()) > closeDistance;
    }

    public void SetPoints(LowPolyPointHandle A, LowPolyPointHandle B)
    {
        if(PtA != A)
        {
            RemovePointListeners(PtA);
            AddPointListeners(A);
            PtA = A;
        }

        if (PtB != B)
        {
            RemovePointListeners(PtB);
            AddPointListeners(B);
            PtB = B;
        }
    }
    
    private Vector3 GetMiddlePosition()
    {
        if (PtA != null && PtB != null)
            return (PtA.transform.position + PtB.transform.position) / 2f;
        else
            return transform.position;
    }

    public void UpdatePosition()
    {
        transform.position = GetMiddlePosition();
    }

    private void OnMovePoint(LowPolyPointHandle ptHandle)
    {
        UpdatePosition();
    }

    private void OnFusionPoint(LowPolyPointHandle ptHandle)
    {
        LowPolyPointHandle fusiontWith = ptHandle.CloseHandle;

        if (fusiontWith == PtA)
            SetPoints(fusiontWith, PtB);
        else if (fusiontWith == PtB)
            SetPoints(PtA, fusiontWith);
        else
            DestroyImmediate(this.gameObject);
    }

    private void OnDestroyPoint(LowPolyPointHandle ptHandle)
    {
        DestroyImmediate(this.gameObject);
    }

    public void AddMoveAction(UnityAction<LowPolySegmentHandle> action)
    {
        if (action == null || moveActions.Contains(action))
            return;

        OnMoved.AddListener(action);
        moveActions.Add(action);
    }

    public void RemoveMoveAction(UnityAction<LowPolySegmentHandle> action)
    {
        if (action == null) return;

        if (OnMoved != null)
            OnMoved.RemoveListener(action);

        if (moveActions != null)
            moveActions.Remove(action);
    }

    private void AddPointListeners(LowPolyPointHandle pt)
    {
        if (pt != null)
        {
            pt.AddMoveAction(OnMovePoint);
            pt.AddFusionAction(OnFusionPoint);
            pt.AddDestroyAction(OnDestroyPoint);
        }
    }

    private void RemovePointListeners(LowPolyPointHandle pt)
    {
        if (pt != null)
        {
            pt.RemoveMoveAction(OnMovePoint);
            pt.RemoveFusionAction(OnFusionPoint);
            pt.RemoveDestroyAction(OnDestroyPoint);
        }
    }

    public void RemoveAllListeners()
    {
        if(OnMoved != null)
            OnMoved.RemoveAllListeners();

        if (moveActions != null)
            moveActions.Clear();

        RemovePointListeners(PtA);
        RemovePointListeners(PtB);
    }
}
