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
        FollowCamera.FollowSettings settings = GetWorldSettings();

        Color drawingColor = Color.white;
        drawingColor.a = .05f;
        Gizmos.color = drawingColor;
        Gizmos.DrawCube(settings.travelingRect.center, settings.travelingRect.size);
        
        Gizmos.color = Color.white;
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
        settings.targetPosition.x = settings.followX ? settings.target.position.x : settings.targetPosition.x + transform.position.x;
        settings.targetPosition.y = settings.followY ? settings.target.position.y : settings.targetPosition.y + transform.position.y;

        return settings;
    }
}