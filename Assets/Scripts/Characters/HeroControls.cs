using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControls : MonoBehaviour
{
    [Header("Buttons")]
    public bool useButtonControls;
    public string directionAxisName = "Horizontal";
    public string jumpButtonName = "Jump";
    public string actionButtonName = "Fire1";

    private ValueChangeEvent<int> AxisInput;
    private ValueChangeEvent<bool> Action1Input;
    private ValueChangeEvent<bool> Action2Input;

    private TouchScreen touchInput;
    private Pilot pilot;
    private Pilotable.PilotingType currentPilotingType;

    private void Awake()
    {
        AxisInput = new ValueChangeEvent<int>();
        Action1Input = new ValueChangeEvent<bool>();
        Action2Input = new ValueChangeEvent<bool>();

        touchInput = GetComponent<TouchScreen>();
        pilot = GetComponent<Pilot>();
    }

    private void Start()
    {
        if (pilot != null)
        {
            pilot.IsPilotingVehicle.AddListener(OnPilotingChange);
            OnPilotingChange(pilot.IsPilotingVehicle.Value);
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
        if (pilot != null) pilot.IsPilotingVehicle.RemoveListener(OnPilotingChange);
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
        if (axis == 0f) AxisInput.Value = 0;
        else if (axis > 0) AxisInput.Value = 1;
        else AxisInput.Value = -1;

        Action1Input.Value = Input.GetButtonDown(jumpButtonName);
        Action2Input.Value = Input.GetButtonDown(actionButtonName);
    }

    private void GetTouchControl()
    {
        if (touchInput.Holds.Length == 1)
            AxisInput.Value = touchInput.Holds[0].x < Screen.width/2f ? -1 : 1;
        else
            AxisInput.Value = 0;

        Action1Input.Value = touchInput.Taps.Length > 0;
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

            if (Action1Input.Value) blob.jumper.Jump();

            blob.spinner.Spin(AxisInput.Value);

            Action1Input.hasChanged = false;
            Action2Input.hasChanged = false;
            AxisInput.hasChanged = false;

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

            bot.walker.Walk(AxisInput.Value);
            if (Action1Input.Value) bot.jumper.Jump();
            bot.spinner.Spin(AxisInput.Value);

            if (Action2Input.Value) pilot.ExitCurrentVehicle();

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

            if (AxisInput.hasChanged)
            {
                if (AxisInput.Value != 0)
                {
                    if (directionBuffer != 0)
                    {
                        if (AxisInput.Value * directionBuffer > 0)
                            gearBuffer = bug.gearBox.ClampedGear(gearBuffer + 1);
                        else
                        {
                            directionBuffer = 0;
                            gearBuffer = 0;
                        }
                    }
                    else
                        directionBuffer = AxisInput.Value;
                }

                AxisInput.hasChanged = false;
            }

            if (bug.walker.CurrentDirection == Walker.Direction.IDLE || bug.groundProbe.GroundFlatness.Value == (int)FlatGroundProbe.Flatness.Flat)
            {
                bug.walker.Walk(directionBuffer);
                if (bug.gearBox.CurrentGear.Value < gearBuffer) bug.gearBox.GearUp();
                if (Action2Input.Value) pilot.ExitCurrentVehicle();
            }

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

            if (Action1Input.Value) jet.jumper.Jump();
            jet.spinner.Spin(AxisInput.Value);

            if (Action2Input.Value) pilot.ExitCurrentVehicle();

            Action1Input.hasChanged = false;
            Action2Input.hasChanged = false;
            AxisInput.hasChanged = false;

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

            if (Action1Input.Value) buzz.jumper.Jump();
            buzz.spinner.Spin(AxisInput.Value);

            if (Action2Input.Value) pilot.ExitCurrentVehicle();

            Action1Input.hasChanged = false;
            Action2Input.hasChanged = false;
            AxisInput.hasChanged = false;

            yield return null;
        }

        SwitchControls();
    }
}
