using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraRoom))]
public class CameraRoomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Enter"))
        {
            CameraRoom room = target as CameraRoom;
            CameraBrain cam = GameObject.FindObjectOfType<CameraBrain>();

            if (cam != null)
            {
                cam.SetCurrentSettings(room.enterSettings);
                cam.ApplyCurrentSettings();
            }
        }

        if (CameraRoom.showAllRoomsGizmos)
        {
            if (GUILayout.Button("Hide rooms"))
                CameraRoom.showAllRoomsGizmos = false;
        }
        else
        {
            if (GUILayout.Button("Show rooms")) CameraRoom.showAllRoomsGizmos = true;
        }

        EditorGUILayout.EndHorizontal();
    }
}
