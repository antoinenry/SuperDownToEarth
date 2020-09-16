using UnityEngine;

public class HeroBrain : MonoBehaviour, IPausable
{
    public enum InputType { Touch, Buttons, Mouse }

    public InputType activeInput;

    [Header("Touch & Mouse")]
    public bool rotateRelativeToPlayer = false;
    public float aimDistanceMin = 1f;
    public Hysteresis aimDistanceHysteresis;
    public Hysteresis aimAngleHysteresis;
    public Vector2ChangeEvent aimPosition;

    [Header("Buttons")]
    public string directionAxisName = "Horizontal";
    public string action1ButtonName = "Action1";
    public string action2ButtonName = "Action2";

    private TouchControls touchControls;
    private Pilot pilot;
    private PlayerControls currentControls;


    private void Awake()
    {
        touchControls = GetComponent<TouchControls>();
        pilot = GetComponent<Pilot>();
    }

    private void Start()
    {
        Input.simulateMouseWithTouches = false;
    }

    private void OnEnable()
    {
        pilot.currentVehicle.AddValueListener<Component>(OnControlsChange);
    }

    private void OnDisable()
    {
        pilot.currentVehicle.RemoveValueListener<Component>(OnControlsChange);
    }

    private void Update()
    {
        DetectControlType();

        if (currentControls != null)
        {
            switch (activeInput)
            {
                case InputType.Touch:
                    GetTouchControls();
                    break;
                case InputType.Buttons:
                    GetButtonControls();
                    break;
                case InputType.Mouse:
                    GetMouseControls();
                    break;
            }
        }
    }

    private void OnControlsChange(Object piloted)
    {
        if (currentControls != null) currentControls.ResetControls();

        if (piloted != null && piloted is Body)
        {
            currentControls = (piloted as Body).GetComponentInChildren<PlayerControls>();
            currentControls.enabled = true;
        }
        else
        {
            currentControls = null;
        }
    }

    private void DetectControlType()
    {
        if (Input.touchCount > 0) activeInput = InputType.Touch;
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) activeInput = InputType.Mouse;
        else if (Input.anyKeyDown) activeInput = InputType.Buttons;
    }
    
    private void GetButtonControls()
    {
        aimPosition.Value = Vector2.zero;
        float axis = Input.GetAxisRaw(directionAxisName);

        if (axis == 0f) currentControls.axisInput.Value = 0;
        else if (axis > 0) currentControls.axisInput.Value = 1;
        else currentControls.axisInput.Value = -1;

        if (Input.GetButtonDown(action1ButtonName))
            currentControls.action1Input.Trigger();

        if ((Input.GetButtonDown(action2ButtonName)))
            currentControls.action2Input.Trigger();
    }
        
    private void GetTouchControls()
    {
        bool tap = touchControls.Taps.Length > 0;
        Vector2 tapPosition = tap ? touchControls.Taps[0] : Vector2.zero;
        bool hold = touchControls.Holds.Length > 0;
        Vector2 holdPosition = hold ? touchControls.Holds[0] : Vector2.zero;

        GetScreenRelativeControls(tap, tapPosition, hold, holdPosition);
    }    

    private void GetMouseControls()
    {
        GetScreenRelativeControls(Input.GetMouseButtonDown(0), Input.mousePosition, Input.GetMouseButton(1), Input.mousePosition);
    }

    private void GetScreenRelativeControls(bool tapInput, Vector2 tapPosition, bool holdInput, Vector2 holdPosition)
    {
        int axisInput = 0;
        Vector2 aimInput = Vector2.zero;

        Camera currentCamera = Camera.main;
        if (currentCamera != null)
        {       
            if (holdInput)
            {
                if (rotateRelativeToPlayer)
                {
                    Vector2 worldholdPos = currentCamera.ScreenToWorldPoint(holdPosition);
                    float distanceToHero = Vector2.Distance(transform.position, worldholdPos);

                    aimDistanceHysteresis.Input = distanceToHero / aimDistanceMin;

                    if (aimDistanceHysteresis.Output > 0)
                    {
                        Vector2 heroScreenPosition = currentCamera.WorldToScreenPoint(transform.position);
                        float holdTooHeroAngle = Vector2.SignedAngle(holdPosition - heroScreenPosition, transform.up);
                        aimAngleHysteresis.Input = 1f - Mathf.Abs(Mathf.Abs(holdTooHeroAngle) - 90f) / 90f;
                        axisInput = (int)(aimAngleHysteresis.Output * Mathf.Sign(holdTooHeroAngle));

                        if (axisInput != 0) aimInput = worldholdPos - (Vector2)transform.position;
                    }
                }
                else
                {
                    if (touchControls.touchRight)
                        axisInput = 1;
                    else if(touchControls.touchLeft)
                        axisInput = -1;
                }
            }

            if (tapInput)
            {
                Vector2 worldtapPos = currentCamera.ScreenToWorldPoint(tapPosition);
                float distanceToHero = Vector2.Distance(transform.position, worldtapPos);

                if (distanceToHero > aimDistanceMin)
                    currentControls.action1Input.Trigger();
                else
                    currentControls.action2Input.Trigger();
            }
        }

        currentControls.axisInput.Value = axisInput;
        aimPosition.Value = aimInput;
    }

    public void Pause(bool pause)
    {
        enabled = !pause;
    }
}
