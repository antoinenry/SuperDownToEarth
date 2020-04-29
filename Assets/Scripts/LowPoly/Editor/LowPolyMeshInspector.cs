using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LowPolyMesh))]
public class LowPolyMeshInspector : Editor
{
    private LowPolyMesh lpmTarget;

    private LowPolySubMesh[] currentSubMeshes;
    
    public override void OnInspectorGUI()
    {
        TargetUpdate();
        SubMeshUpdate();
        ShowGUI();
    }

    private void ShowGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Get submeshes in children"))
        {
            lpmTarget.SetSubMeshesFromChildren();
        }

        //GUISortingLayerPopUp();
    }

    private void TargetUpdate()
    {
        if (lpmTarget != target)
        {
            lpmTarget = target as LowPolyMesh;
            currentSubMeshes = new LowPolySubMesh[lpmTarget.SubMeshCount];
            lpmTarget.GetSubMeshes(currentSubMeshes);
        }
    }

    private void SubMeshUpdate()
    {
        if (lpmTarget.CompareSubMeshes(currentSubMeshes) == false)
        {
            lpmTarget.RemoveListeners(currentSubMeshes);

            currentSubMeshes = new LowPolySubMesh[lpmTarget.SubMeshCount];
            lpmTarget.GetSubMeshes(currentSubMeshes);

            lpmTarget.SetSubMeshes(currentSubMeshes);
            lpmTarget.CallMeshUpdate();
        }
    }

    private void GUISortingLayerPopUp()
    {
        MeshRenderer mr = lpmTarget.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            EditorGUILayout.HelpBox("Missing MeshRenderer", MessageType.Info);
            return;
        }

        List<string> layerNames = new List<SortingLayer>(SortingLayer.layers).ConvertAll<string>(x => x.name);
        int layerIndex = layerNames.IndexOf(mr.sortingLayerName);
        int indexSelection = EditorGUILayout.Popup("Sorting layer", layerIndex, layerNames.ToArray());

        if (indexSelection != layerIndex)
        {
            mr.sortingLayerID = SortingLayer.NameToID(layerNames[indexSelection]);
        }
    }
}
