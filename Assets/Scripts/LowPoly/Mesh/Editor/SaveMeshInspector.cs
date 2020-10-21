using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class SaveMeshInspector
{
    public static string lastPath = "Assets/";

    [MenuItem("CONTEXT/MeshFilter/Save mesh")]
    static void SaveMesh(MenuCommand command)
    {
        MeshFilter filter = (MeshFilter)command.context;
        Mesh mesh = filter.sharedMesh;

        if (mesh == null)
        {
            EditorUtility.DisplayDialog("Save mesh as asset", "The MeshFilter doesn't have any mesh data", "Cancel");
            return;
        }
        else
        {
            string savePath = EditorUtility.SaveFilePanel("Save mesh as asset", lastPath, mesh.name, "asset");

            if (savePath.Length != 0)
            {
                lastPath = savePath;
                savePath = FileUtil.GetProjectRelativePath(savePath);
                Object meshAsset = Object.Instantiate(mesh);

                AssetDatabase.CreateAsset(meshAsset, savePath);
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog("Save mesh as asset", "Mesh saved", "Ok");

                filter.sharedMesh = meshAsset as Mesh;

                return;
            }
        }
    }
}
