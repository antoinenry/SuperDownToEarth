using UnityEngine;

public class HeroBrain : MonoBehaviour
{
    public enum InputType { Touch, Buttons, Mouse }

    public InputType activeInput;

    [Header("Buttons")]
    public string directionAxisName = "Horizontal";
    public string action1ButtonName = "Action1";
    public string action2ButtonName = "Action2";

    private TouchControls touchControls;
    private Pilot pilot;
    private PlayerControls currentControls;

    public Vector2ChangeEvent aimPosition;

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
        Camera currentCamera = Camera.main;
        if (currentCamera != null && touchControls.Holds.Length >= 1)
        {
            Vector3 touchHoldPosition = touchControls.Holds[0];
            aimPosition.Value = (Vector2)currentCamera.ScreenToWorldPoint(touchHoldPosition) - (Vector2)transform.position;

            Vector2 heroScreenPosition = currentCamera.WorldToScreenPoint(transform.position);
            float touchToHeroAngle = Vector2.SignedAngle((Vector2)touchHoldPosition - heroScreenPosition, transform.up);
            currentControls.axisInput.Value = touchToHeroAngle > 0 ? 1 : -1;
        }
        else
        {
            currentControls.axisInput.Value = 0;
            aimPosition.Value = Vector2.zero;
        }

        if (touchControls.Taps.Length > 0)
            currentControls.action1Input.Trigger();
    }    

    private void GetMouseControls()
    {
        Camera currentCamera = Camera.main;
        if (currentCamera != null && Input.GetMouseButton(1))
        {
            Vector3 mousePos = Input.mousePosition;
            aimPosition.Value = (Vector2)currentCamera.ScreenToWorldPoint(mousePos) - (Vector2)transform.position;

            Vector2 heroScreenPosition = currentCamera.WorldToScreenPoint(transform.position);
            float mouseToHeroAngle = Vector2.SignedAngle((Vector2)mousePos - heroScreenPosition, transform.up);
            currentControls.axisInput.Value = mouseToHeroAngle > 0 ? 1 : -1;
        }
        else
        {
            currentControls.axisInput.Value = 0;
            aimPosition.Value = Vector2.zero;
        }

        if (Input.GetMouseButtonUp(0))
            currentControls.action1Input.Trigger();
    }
}
