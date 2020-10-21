using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEditor;


public class LowPolyWorldEditor : EditorWindow
{
    private List<LowPolyShapeHandler> shapeHandlers = new List<LowPolyShapeHandler>();
    private int shapeCounter = 0;
    private GameObject shapeModel;

    [MenuItem("Window/LowPolyWorld Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LowPolyWorldEditor));        
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Show handles"))
        {
            RemoveMissingShapeHandlers();
            shapeHandlers = new List<LowPolyShapeHandler>(GameObject.FindObjectsOfType<LowPolyShapeHandler>());

            Component[] selectedShapeComponents = LowPolyShape.GetShapeComponentsInGameObjects(Selection.gameObjects, true);

            if (selectedShapeComponents != null)
            {
                foreach (Component shapeComponent in selectedShapeComponents)
                    HandleShape(shapeComponent);
            }
        }

        if (GUILayout.Button("Hide handles"))
        {
            shapeHandlers = new List<LowPolyShapeHandler>(GameObject.FindObjectsOfType<LowPolyShapeHandler>());

            DeleteHandlers();
        }

        shapeModel = EditorGUILayout.ObjectField("Shape model", shapeModel, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("Create shape from handles"))
        {
            CreateShapeFromHandles();
        }
    }
    
    private void HandleShape(Component shapeComponent)
    {
        if (shapeComponent == null) return;
        
        if (shapeHandlers == null) shapeHandlers = new List<LowPolyShapeHandler>();        

        LowPolyShapeHandler shapeHandler = shapeHandlers.Find(h => h.Shape == shapeComponent);
        if (shapeHandler == null)
        {
            shapeHandler = LowPolyShapeHandler.Instantiate(shapeComponent);
            shapeHandler.name += "_" + shapeCounter;

            shapeHandlers.Add(shapeHandler);
            shapeCounter++;
        }
        else
        {
            shapeHandler.UpdatePointHandles();
        }
    }

    private void DeleteHandlers()
    {
        shapeCounter = 0;
        if (shapeHandlers != null)
        {
            foreach (LowPolyShapeHandler handler in shapeHandlers)
            {
                if (handler.IsPermanent())
                {
                    handler.DestroyTemporaryHandles();
                }
                else
                {
                    DestroyImmediate(handler.gameObject);
                    shapeHandlers = null;
                }
            }
        }
    }

    private void RemoveMissingShapeHandlers()
    {
        if (shapeHandlers == null) return;
        for (int i = shapeHandlers.Count - 1; i >= 0; i--)
        {
            if (shapeHandlers[i] == null || shapeHandlers[i].gameObject == null) shapeHandlers.RemoveAt(i);
        }
    }

    private void CreateShapeFromHandles()
    {
        if (shapeModel == null)
            return;

        Component[] shapeModelComponents = LowPolyShape.GetShapeComponentsInGameObject(shapeModel, false);
        if (shapeModelComponents == null || shapeModelComponents.Length == 0)
            return;

        List<LowPolyPointHandle> selectedHandles = new List<LowPolyPointHandle>(GetSelectedHandles());
        if (selectedHandles.Count == 0)
            return;

        Vector3[] handlePositions = selectedHandles.ConvertAll<Vector3>(x => x.transform.position).ToArray();

        GameObject newShapeObject = Instantiate<GameObject>(shapeModel);
        Component[] newShapeComponents = LowPolyShape.GetShapeComponentsInGameObject(newShapeObject, false);

        LowPolyShape lps = LowPolyShape.New(newShapeComponents[0]);
        lps.SetPositions(handlePositions);
        lps.SortPositions();
        //lps.SetTransformInCenter();

        for (int i = 1; i < newShapeComponents.Length; i++)
        {
            lps = LowPolyShape.New(newShapeComponents[i]);
            lps.CopyPositionFromOtherShapeComponent(newShapeComponents[0]);
        }
    }

    private LowPolyPointHandle[] GetSelectedHandles()
    {
        List <LowPolyPointHandle> selectedHandles = new List<LowPolyPointHandle>();

        if (Selection.gameObjects != null)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                LowPolyPointHandle h = go.GetComponent<LowPolyPointHandle>();
                if (h != null)
                    selectedHandles.Add(h);
            }
        }

        return selectedHandles.ToArray();
    }

    [MenuItem("CONTEXT/LineRenderer/Auto set shape")]
    static void AutoSetLine(MenuCommand command)
    {
        LineRenderer line = (LineRenderer)command.context;
        LowPolyShape.New(line).CopyPositionFromOtherShapeComponent();
    }

    [MenuItem("CONTEXT/LowPolySubMesh/Auto set shape")]
    static void AutoSetSubMesh(MenuCommand command)
    {
        LowPolySubMesh sub = (LowPolySubMesh)command.context;
        LowPolyShape.New(sub).CopyPositionFromOtherShapeComponent();
    }

    [MenuItem("CONTEXT/PolygonCollider2D/Auto set shape")]
    static void AutoSetPolygonCollider(MenuCommand command)
    {
        PolygonCollider2D collider = (PolygonCollider2D)command.context;
        LowPolyShape.New(collider).CopyPositionFromOtherShapeComponent();
    }
}
