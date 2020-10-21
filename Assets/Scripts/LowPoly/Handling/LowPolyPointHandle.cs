using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class LowPolyPointHandle : MonoBehaviour
{
    public class HandleEvent : UnityEvent<LowPolyPointHandle> {}

    public float closeDistance = 1f;
    public bool permanent;

    private bool drag;

    public LowPolyPointHandle CloseHandle { get; private set; }
    public Vector3 LastPosition { get; private set; }

    private HandleEvent OnMoved;
    private HandleEvent OnDestroyed;
    private HandleEvent OnFusion;

    [SerializeField] private List<UnityAction<LowPolyPointHandle>> moveActions;
    [SerializeField] private List<UnityAction<LowPolyPointHandle>> destroyActions;
    [SerializeField] private List<UnityAction<LowPolyPointHandle>> fusionActions;

    public static LowPolyPointHandle InstantiateHandle(Vector3 position, string handleName = "pointHandle")
    {
        LowPolyPointHandle newHandle = new GameObject().AddComponent<LowPolyPointHandle>();
        newHandle.transform.position = position;
        newHandle.name = handleName;

        return newHandle;
    }

    private void OnDrawGizmos()
    {
        if(drag)
        {
            if (CloseHandle == null)
                Gizmos.DrawIcon(transform.position, "LowPolyEditor/Handle2", true);
            else
                Gizmos.DrawIcon(CloseHandle.transform.position, "LowPolyEditor/Handle2", true);
        }
        else
        {
            if (permanent)
                Gizmos.DrawIcon(transform.position, "LowPolyEditor/Handle4", true);
            else
                Gizmos.DrawIcon(transform.position, "LowPolyEditor/Handle1", true);
        }
    }

    private void Awake()
    {
        if (OnMoved == null) OnMoved = new HandleEvent();
        if (OnDestroyed == null) OnDestroyed = new HandleEvent();
        if (OnFusion == null) OnFusion = new HandleEvent();
        if (moveActions == null) moveActions = new List<UnityAction<LowPolyPointHandle>>();
        if (destroyActions == null) destroyActions = new List<UnityAction<LowPolyPointHandle>>();
        if (fusionActions == null) fusionActions = new List<UnityAction<LowPolyPointHandle>>();
    }

    private void Update()
    {
        if (permanent == false && moveActions.Count == 0 && destroyActions.Count == 0)
        {
            DestroyImmediate(gameObject);
            return;
        }

        if (transform.hasChanged)
        {
            OnMoved.Invoke(this);
            LastPosition = transform.position;
            transform.hasChanged = false;
        }
    }

    public bool Drag
    {
        get { return drag; }
        set
        {
            drag = value;
            if (drag)
                TryFindCloseHandle();
            else
                FusionWithCloseHandle();
        }
    }

    private void TryFindCloseHandle()
    {
        LowPolyPointHandle[] allHandles = GameObject.FindObjectsOfType<LowPolyPointHandle>();

        foreach (LowPolyPointHandle other in allHandles)
        {
            if (other != this && Vector2.Distance(transform.position, other.transform.position) < closeDistance)
            {
                CloseHandle = other;
                return;
            }
        }

        CloseHandle = null;
    }

    public void FusionWithCloseHandle()
    {
        if (CloseHandle == null) return;

        OnFusion.Invoke(this);
        CloseHandle.OnMoved.Invoke(CloseHandle);

        foreach (UnityAction<LowPolyPointHandle> action in moveActions)
            CloseHandle.AddMoveAction(action);
        foreach (UnityAction<LowPolyPointHandle> action in destroyActions)
            CloseHandle.AddDestroyAction(action);
        foreach (UnityAction<LowPolyPointHandle> action in fusionActions)
            CloseHandle.AddFusionAction(action);

        RemoveAllListeners();
        DestroyImmediate(gameObject);
    }

    public void AddMoveAction(UnityAction<LowPolyPointHandle> action)
    {
        if (action == null || moveActions.Contains(action)) return;

        OnMoved.AddListener(action);
        moveActions.Add(action);
    }

    public void RemoveMoveAction(UnityAction<LowPolyPointHandle> action)
    {
        if (action == null) return;

        if (OnMoved != null)
            OnMoved.RemoveListener(action);

        if (moveActions != null)
            moveActions.Remove(action);
    }

    public void AddDestroyAction(UnityAction<LowPolyPointHandle> action)
    {
        if (action == null || destroyActions.Contains(action)) return;

        OnDestroyed.AddListener(action);
        destroyActions.Add(action);
    }

    public void RemoveDestroyAction(UnityAction<LowPolyPointHandle> action)
    {
        if (action == null) return;

        if (OnDestroyed != null)
            OnDestroyed.RemoveListener(action);

        if(destroyActions != null)
            destroyActions.Remove(action);
    }

    public void AddFusionAction(UnityAction<LowPolyPointHandle> action)
    {
        if (action == null || fusionActions.Contains(action)) return;

        OnFusion.AddListener(action);
        fusionActions.Add(action);
    }

    public void RemoveFusionAction(UnityAction<LowPolyPointHandle> action)
    {
        if (action == null) return;

        if (OnFusion != null)
            OnFusion.RemoveListener(action);

        if (fusionActions != null)
            fusionActions.Remove(action);
    }

    public void RemoveAllListeners()
    {
        if(OnDestroyed != null)
            OnDestroyed.RemoveAllListeners();
        if (OnMoved != null)
            OnMoved.RemoveAllListeners();
        if (OnFusion != null)
            OnFusion.RemoveAllListeners();

        if (destroyActions != null)
            destroyActions.Clear();
        if (moveActions != null)
            moveActions.Clear();
        if (fusionActions != null)
            fusionActions.Clear();

    }

    private void OnDestroy()
    {
        if (OnDestroyed != null)
            OnDestroyed.Invoke(this);

        RemoveAllListeners();
    }
}
