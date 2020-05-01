using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControls : MonoBehaviour, IValueChangeEventsComponent
{
    [Header("Buttons")]
    public bool useButtonControls;
    public string directionAxisName = "Horizontal";
    public string jumpButtonName = "Jump";
    public string actionButtonName = "Fire1";
    
    private TouchScreen touchInput;
    private Pilot pilot;
    private Pilotable.PilotingType currentPilotingType;

    public ValueChangeEvent AxisInput = ValueChangeEvent.NewValueChangeEvent<int>();
    public ValueChangeEvent Action1Input = ValueChangeEvent.NewTriggerEvent();
    public ValueChangeEvent Action2Input = ValueChangeEvent.NewTriggerEvent();
    
    public int GetValueChangeEvents(out ValueChangeEvent[] vces)
    {
        vces = new ValueChangeEvent[] { AxisInput, Action1Input, Action2Input };
        return vces.Length;
    }

    public void SetValueChangeEventsID()
    {
        AxisInput.SetID("Axis1Input", this, 0);
        Action1Input.SetID("Action1Input", this, 1);
        Action2Input.SetID("Action2Input", this, 2);
    }

    public void EnslaveValueChangeEvents(bool enslave)
    {
        AxisInput.Enslave<int>(enslave);
        Action1Input.Enslave(enslave);
        Action2Input.Enslave(enslave);
    }

    private void Awake()
    {
        touchInput = GetComponent<TouchScreen>();
        pilot = GetComponent<Pilot>();
    }

    private void Start()
    {
        if (pilot != null)
        {
            pilot.IsPilotingVehicle.AddListener<bool>(OnPilotingChange);
            OnPilotingChange(pilot.IsPilotingVehicle.GetValue<bool>());
        }

        StopAllCoroutines();
        SwitchControls();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.B)) Debug.Break();
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        SwitchControls();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        if (pilot != null) pilot.IsPilotingVehicle.RemoveListener<bool>(OnPilotingChange);
    }

    private void OnPilotingChange(bool isPiloting)
    {
        Body newPilotedBody = isPiloting ? pilot.CurrentVehicle : pilot.body;

        if (newPilotedBody != null && newPilotedBody.pilotableConfig != null)
            currentPilotingType = newPilotedBody.pilotableConfig.GetPilotingType();
        else
            currentPilotingType = Pilotable.PilotingType.None;
    }

    private void GetButtonControl()
    {
        float axis = Input.GetAxisRaw(directionAxisName);
        if (axis == 0f) AxisInput.SetValue(0);
        else if (axis > 0) AxisInput.SetValue(1);
        else AxisInput.SetValue(-1);
        
        if (Input.GetButtonDown(jumpButtonName))
            Action1Input.Invoke();

        if ((Input.GetButtonDown(actionButtonName)))
            Action2Input.Invoke();
    }

    private void GetTouchControl()
    {
        if (touchInput.Holds.Length == 1)
            AxisInput.SetValue(touchInput.Holds[0].x < Screen.width/2f ? -1 : 1);
        else
            AxisInput.SetValue(0);

        if ((touchInput.Taps.Length > 0))
            Action1Input.Invoke();
    }

    private void SwitchControls()
    {        
        switch(currentPilotingType)
        {
            case Pilotable.PilotingType.Blob: StartCoroutine(BlobControlsCoroutine());
                break;
            case Pilotable.PilotingType.Bot: StartCoroutine(BotControlsCoroutine());
                break;
            case Pilotable.PilotingType.Bug: StartCoroutine(BugControlsCoroutine());
                break;
            case Pilotable.PilotingType.Jet: StartCoroutine(JetControlsCoroutine());
                break;                
            case Pilotable.PilotingType.Buzz: StartCoroutine(BuzzControlsCoroutine());
                break;
        }
    }

    private IEnumerator BlobControlsCoroutine()
    {
        PilotableBlob blob = pilot.PilotedBody.pilotableConfig as PilotableBlob;

        while (currentPilotingType == Pilotable.PilotingType.Blob)
        {
            if (useButtonControls) GetButtonControl();
            else GetTouchControl();

            if (Action1Input.Invoked) blob.jumper.Jump();

            blob.spinner.Spin(AxisInput.GetValue<int>());

            Action1Input.Invoked = false;
            Action2Input.Invoked = false;
            AxisInput.Invoked = false;
            yield return null;
        }

        SwitchControls();
    }

    private IEnumerator BotControlsCoroutine()
    {
        PilotableBot bot = pilot.PilotedBody.pilotableConfig as PilotableBot;

        while (currentPilotingType == Pilotable.PilotingType.Bot)
        {
            if (useButtonControls) GetButtonControl();
            else GetTouchControl();

            bot.walker.Walk(AxisInput.GetValue<int>());
            if (Action1Input.Invoked) bot.jumper.Jump();
            bot.spinner.Spin(AxisInput.GetValue<int>());

            if (Action2Input.Invoked) pilot.ExitCurrentVehicle();

            Action1Input.Invoked = false;
            Action2Input.Invoked = false;
            AxisInput.Invoked = false;
            yield return null;
        }

        SwitchControls();
    }

    private IEnumerator BugControlsCoroutine()
    {
        PilotableBug bug = pilot.PilotedBody.pilotableConfig as PilotableBug;
        int directionBuffer = 0;
        int gearBuffer = 0;

        while (currentPilotingType == Pilotable.PilotingType.Bug)
        {
            if (useButtonControls) GetButtonControl();
            else GetTouchControl();

            if (AxisInput.Invoked)
            {
                int axis = AxisInput.GetValue<int>();
                if (axis != 0)
                {
                    if (directionBuffer != 0)
                    {
                        if (axis * directionBuffer > 0)
                            gearBuffer = bug.gearBox.ClampedGear(gearBuffer + 1);
                        else
                        {
                            directionBuffer = 0;
                            gearBuffer = 0;
                        }
                    }
                    else
                        directionBuffer = axis;
                }
            }

            if (bug.walker.CurrentDirection == Walker.Direction.IDLE || bug.groundProbe.GroundFlatness.GetValue<int>() == (int)FlatGroundProbe.Flatness.Flat)
            {
                bug.walker.Walk(directionBuffer);
                if (bug.gearBox.CurrentGear.GetValue<int>() < gearBuffer) bug.gearBox.GearUp();
                if (Action2Input.Invoked) pilot.ExitCurrentVehicle();
            }

            Action1Input.Invoked = false;
            Action2Input.Invoked = false;
            AxisInput.Invoked = false;
            yield return null;
        }

        SwitchControls();
    }

    private IEnumerator JetControlsCoroutine()
    {
        PilotableJet jet = pilot.PilotedBody.pilotableConfig as PilotableJet;

        while (currentPilotingType == Pilotable.PilotingType.Jet)
        {
            if (useButtonControls) GetButtonControl();
            else GetTouchControl();

            if (Action1Input.Invoked) jet.jumper.Jump();
            jet.spinner.Spin(AxisInput.GetValue<int>());

            if (Action2Input.Invoked) pilot.ExitCurrentVehicle();
            
            Action1Input.Invoked = false;
            Action2Input.Invoked = false;
            AxisInput.Invoked = false;
            yield return null;
        }

        SwitchControls();
    }
    
    private IEnumerator BuzzControlsCoroutine()
    {
        PilotableBuzz buzz = pilot.PilotedBody.pilotableConfig as PilotableBuzz;

        while (currentPilotingType == Pilotable.PilotingType.Buzz)
        {
            if (useButtonControls) GetButtonControl();
            else GetTouchControl();

            if (Action1Input.Invoked) buzz.jumper.Jump();
            buzz.spinner.Spin(AxisInput.GetValue<int>());

            if (Action2Input.Invoked) pilot.ExitCurrentVehicle();

            Action1Input.Invoked = false;
            Action2Input.Invoked = false;
            AxisInput.Invoked = false;
            yield return null;
        }

        SwitchControls();
    }
}
