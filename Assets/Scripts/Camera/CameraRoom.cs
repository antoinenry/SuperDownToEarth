using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraRoom : MonoBehaviour
{
    public CameraSettings enterSettings;

    public static bool showAllRoomsGizmos;

    private void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

    private void OnDrawGizmos()
    {
        if (showAllRoomsGizmos) DrawGizmos();
    }

    private void DrawGizmos()
    {
        Vector3 enterPos = transform.position;
        if (enterSettings.positionOffset.action == CameraSettings.ActionType.UseValue)
            enterPos += (Vector3)enterSettings.positionOffset.value;

        Gizmos.color = Color.blue;
        Gizmos.DrawIcon(enterPos, "CameraRoom/eye", true);

        float enterSize = enterSettings.orthographicSize.value;
        Gizmos.color = new Color(0f, 0f, 1f, .1f);
        Gizmos.DrawCube(enterPos, new Vector3(2 * enterSize * Camera.main.aspect, 2 * enterSize, 1f));
    }
}