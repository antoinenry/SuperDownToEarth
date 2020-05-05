using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControls : MonoBehaviour, IValueChangeEventsComponent
{
    public string heroTag = "Player";
    public bool useButtonControls;

    [Header("Buttons")]
    public string directionAxisName = "Horizontal";
    public string jumpButtonName = "Jump";
    public string actionButtonName = "Fire1";
    
    private TouchScreen touchInput;

    public ValueChangeEvent AxisInput = ValueChangeEvent.New<int>();
    public ValueChangeEvent Action1Input = ValueChangeEvent.New<trigger>();
    public ValueChangeEvent Action2Input = ValueChangeEvent.New<trigger>();

    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { AxisInput, Action1Input, Action2Input };
        return vces.Length;
    }

    public int SetValueChangeEventsID()
    {
        AxisInput.SetID("Axis1Input", this, 0);
        Action1Input.SetID("Action1Input", this, 1);
        Action2Input.SetID("Action2Input", this, 2);
        return 3;
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        AxisInput.Enslave(enslave);
        Action1Input.Enslave(enslave);
        Action2Input.Enslave(enslave);
    }

    private void Awake()
    {
        touchInput = GetComponent<TouchScreen>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (useButtonControls)
            GetButtonControl();
        else
            GetTouchControls();
    }

    private void GetButtonControl()
    {
        float axis = Input.GetAxisRaw(directionAxisName);

        if (axis == 0f) AxisInput.Set(0);
        else if (axis > 0) AxisInput.Set(1);
        else AxisInput.Set(-1);
        
        if (Input.GetButtonDown(jumpButtonName))
            Action1Input.Invoke();

        if ((Input.GetButtonDown(actionButtonName)))
            Action2Input.Invoke();
    }

    private void GetTouchControls()
    {
        if (touchInput.Holds.Length == 1)
            AxisInput.Set(touchInput.Holds[0].x < Screen.width/2f ? -1 : 1);
        else
            AxisInput.Set(0);

        if ((touchInput.Taps.Length > 0))
            Action1Input.Invoke();
    }
}
