﻿using UnityEngine;

public class HeroBrain : MonoBehaviour
{
    [Header("Buttons")]
    public string directionAxisName = "Horizontal";
    public string jumpButtonName = "Jump";
    public string actionButtonName = "Fire1";

    //private TouchControls touchInput;
    private Pilot pilot;
    private PlayerControls currentControls;

    private void Awake()
    {
        pilot = GetComponent<Pilot>();
    }

    private void OnEnable()
    {
        pilot.PilotedBody.AddValueListener<Object>(OnControlsChange);
    }

    private void OnDisable()
    {
        pilot.PilotedBody.RemoveValueListener<Object>(OnControlsChange);
    }

    private void Update()
    {
        if (currentControls != null)
            GetButtonControls();
    }

    private void OnControlsChange(Object piloted)
    {
        if (piloted != null && piloted is Body)
            currentControls = (piloted as Body).GetComponent<PlayerControls>();
        else
            currentControls = null;
    }
    
    private void GetButtonControls()
    {
        float axis = Input.GetAxisRaw(directionAxisName);

        if (axis == 0f) currentControls.axisInput.Value = 0;
        else if (axis > 0) currentControls.axisInput.Value = 1;
        else currentControls.axisInput.Value = -1;

        if (Input.GetButtonDown(jumpButtonName))
            currentControls.action1Input.Trigger();

        if ((Input.GetButtonDown(actionButtonName)))
            currentControls.action2Input.Trigger();
    }
    
    /*
    private void GetTouchControls()
    {
        if (touchInput.Holds.Length == 1)
            AxisInput.Set(touchInput.Holds[0].x < Screen.width / 2f ? -1 : 1);
        else
            AxisInput.Set(0);

        if ((touchInput.Taps.Length > 0))
            Action1Input.Invoke();
    }
    */
}