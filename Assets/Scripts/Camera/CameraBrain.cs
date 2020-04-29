using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraBrain : MonoBehaviour
{
    public string actorTag = "Player";
    [Range(0f, 1f)] public float reactionDelay;

    [Header("Camera settings")]
    public CameraSettingsValues defaultSettings;
    public CameraSettingsValues currentSettings;

    [SerializeField] private bool initialized;
    private bool settingsHaveChanged;
    private float reactionTimer;
    private CameraSettingsValues nextSettings;

    private Camera cam;
    private InsideSwitch insideSwitch;
    private CameraActor actor;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>(true);
        insideSwitch = GetComponent<InsideSwitch>();

        if (initialized == false)
        {
            InitDefaultSettings();
            SetAllSettingToDefault();

            initialized = true;
        }
    }
    
    private void Start()
    {
        if (Application.isPlaying)
        {
            GameObject actorGO = GameObject.FindGameObjectWithTag(actorTag);
            if (actorGO != null) SetActor(actorGO.GetComponentInChildren<CameraActor>());
        }
    }
    
    private void Update()
    {        
        if(settingsHaveChanged)
        {
            reactionTimer += Time.deltaTime;
            if (reactionTimer >= reactionDelay)
            {
                currentSettings = nextSettings;
                settingsHaveChanged = false;
                reactionTimer = 0f;
            }
        }

        ApplyCurrentSettings();
    }

    private void InitDefaultSettings()
    {
        GameObject actorGO = GameObject.FindGameObjectWithTag(actorTag);
        if (actorGO != null) defaultSettings.target = actorGO.transform;
        if (cam != null) defaultSettings.orthographicSize = cam.orthographicSize;
    }

    public void SetAllSettingToDefault(bool immediate = true)
    {
        if (immediate)
            currentSettings = defaultSettings;
        else
        {
            nextSettings = defaultSettings;
            settingsHaveChanged = true;
        }
    }

    public void SetCurrentSettings(CameraSettings newSettings)
    {
        currentSettings = CameraSettingsValues(newSettings);
    }

    public void ApplyCurrentSettings()
    {
        if (currentSettings.target != null)
            transform.position = currentSettings.target.position + (Vector3)currentSettings.positionOffset;

        cam.orthographicSize = currentSettings.orthographicSize;
    }

    private CameraSettingsValues CameraSettingsValues(CameraSettings settings)
    {
        CameraSettingsValues values = currentSettings;
        
        switch (settings.target.action)
        {
            case CameraSettings.ActionType.UseDefault: values.target = defaultSettings.target; break;
            case CameraSettings.ActionType.UseValue: values.target = settings.target.value; break;
        }

        switch (settings.positionOffset.action)
        {
            case CameraSettings.ActionType.UseDefault: values.positionOffset = defaultSettings.positionOffset; break;
            case CameraSettings.ActionType.UseValue: values.positionOffset = settings.positionOffset.value; break;
        }

        switch (settings.orthographicSize.action)
        {
            case CameraSettings.ActionType.UseDefault: values.orthographicSize = defaultSettings.orthographicSize; break;
            case CameraSettings.ActionType.UseValue: values.orthographicSize = settings.orthographicSize.value; break;
        }

        return values;
    }

    public void SetActor(CameraActor newActor)
    {
        if (actor != null)
        {
            actor.OnCameraChange.RemoveListener(OnActorChangesRoom);
            actor.OnGetInOut.RemoveListener(OnActorGetsInOut);
        }

        actor = newActor;

        if (actor != null)
        {
            actor.OnCameraChange.AddListener(OnActorChangesRoom);
            actor.OnGetInOut.AddListener(OnActorGetsInOut);
        }
    }

    private void OnActorChangesRoom(CameraSettings roomSettings)
    {
        nextSettings = CameraSettingsValues(roomSettings);
        settingsHaveChanged = true;
    }

    private void OnActorGetsInOut(bool inside)
    {
        if(insideSwitch != null)
            insideSwitch.showInside = inside;
    }
}