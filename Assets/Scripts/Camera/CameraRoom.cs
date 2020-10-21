using UnityEngine;

[ExecuteAlways]
public class CameraRoom : MonoBehaviour
{
    public bool targetIncoming = true;
    public FollowCamera.FollowSettings enterSettings;
    public Trigger enterRoom;

    private FollowCamera followCam;

    private void OnDrawGizmos()
    {
        DrawGizmo(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmo(true);
    }

    private void DrawGizmo(bool selected)
    {
        FollowCamera.FollowSettings settings = GetWorldSettings();

        Color drawingColor = selected ? Color.green : Color.white;
        drawingColor.a = .15f;
        Gizmos.color = drawingColor;
        Gizmos.DrawCube(settings.travelingRect.center, settings.travelingRect.size);

        drawingColor.a = 1f;
        Gizmos.color = drawingColor;
        Vector2 halfCamSize = new Vector2(settings.orthographicSize * Camera.main.aspect, settings.orthographicSize);
        Rect cameraFrame = new Rect((Vector2)transform.position - halfCamSize, halfCamSize * 2f);
        Gizmos.DrawWireCube(cameraFrame.center, cameraFrame.size);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube((Vector2)transform.position, settings.neutralZoneSize);
    }

    private void Awake()
    {
        followCam = Camera.main.GetComponent<FollowCamera>();
        enterSettings.target = transform;
    }

    private void OnEnable()
    {
        if (enterRoom == null) enterRoom = new Trigger();
        enterRoom.AddTriggerListener(OnEnterRoom);
    }

    private void OnDisable()
    {
        enterRoom.RemoveTriggerListener(OnEnterRoom);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetIncoming) enterSettings.target = collision.transform;
        enterRoom.Trigger();
    }

    private void OnEnterRoom()
    {
        followCam.currentSettings = GetWorldSettings();

        if (Application.isPlaying == false)
        {
            followCam.Snap();
        }
    }

    public FollowCamera.FollowSettings GetWorldSettings()
    {
        FollowCamera.FollowSettings settings = enterSettings;
        settings.travelingRect.position += (Vector2)transform.position;
        
        settings.targetPosition.x = settings.followX && settings.target != null ? settings.target.position.x : settings.targetPosition.x + transform.position.x;
        settings.targetPosition.y = settings.followY && settings.target != null ? settings.target.position.y : settings.targetPosition.y + transform.position.y;

        return settings;
    }
}