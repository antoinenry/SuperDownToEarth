using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControls : ValueChangeEventsBehaviour
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

    private void Awake()
    {
        touchInput = GetComponent<TouchScreen>();
    }

    private void Start()
    {
        Action1Input.AddListener(OnAction1Input);
    }

    private void OnAction1Input()
    {
        Debug.Log("Action 1");
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
