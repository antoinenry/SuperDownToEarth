using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Door))]
public class DoorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (Application.isPlaying == false)
            EditorUpdate();
    }
    
    private void EditorUpdate()
    {
        Door targetDoor = target as Door;
        if (targetDoor.movingPart == null) return;

        bool open = targetDoor.isOpen;
        targetDoor.movingPart.localPosition = open ? targetDoor.openPosition : targetDoor.closedPosition;
        targetDoor.movingPart.localRotation = Quaternion.Euler(0f, 0f, open ? targetDoor.openRotation : targetDoor.closedRotation);
    }
}
