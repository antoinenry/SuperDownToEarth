using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LowPolyPointHandle))]
[CanEditMultipleObjects]
public class LowPolyPointHandleInspector : Editor
{
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnScene;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnScene;
    }

    private void OnScene(SceneView view)
    {
        if (Event.current != null)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDrag:
                    DragHandles(true);
                    break;

                case EventType.MouseUp:
                    DragHandles(false);
                    break;
            }
        }
    }

    private void DragHandles(bool drag)
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            LowPolyPointHandle handle = go.GetComponent<LowPolyPointHandle>();
            if (handle != null) handle.Drag = drag;
        }
    }
}
