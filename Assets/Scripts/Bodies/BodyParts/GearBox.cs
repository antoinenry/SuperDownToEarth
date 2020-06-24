using UnityEngine;

[ExecuteAlways]
public class GearBox : MonoBehaviour
{
    public enum GearCycle { Clamp, Loop, Mirror }

    [Header("Settings")]
    public float[] speeds;
    public GearCycle cycleType;
    public bool downMeansStop = true;

    [Header("Controls")]
    public BoolChangeEvent canSwitchGear;
    public IntChangeEvent currentGear;
    public FloatChangeEvent currentSpeed;
    public IntChangeEvent gearSwitch;

    private void Awake()
    {
        if (currentGear == null) currentGear = new IntChangeEvent();
        if (currentSpeed == null) currentSpeed = new FloatChangeEvent();
        if (gearSwitch == null) gearSwitch = new IntChangeEvent();
    }

    private void OnEnable()
    {
        currentGear.AddValueListener<int>(OnGearSet);
        gearSwitch.AddValueListener<int>(OnGearSwitch);
    }

    private void OnDisable()
    {
        currentGear.RemoveValueListener<int>(OnGearSet);
        gearSwitch.RemoveValueListener<int>(OnGearSwitch);
    }

    public int GearCount { get => speeds == null ? 0 : speeds.Length; }
    
    public int CorrectGear(int gear)
    {
        int numGears = GearCount;
        if (numGears != 0)
        {
            switch (cycleType)
            {
                case GearCycle.Clamp:
                    return Mathf.Clamp(gear, 0, numGears - 1);
                case GearCycle.Loop:
                    return (int)Mathf.Repeat(gear, numGears);
                case GearCycle.Mirror:
                    return Mathf.Clamp(gear, 1 - numGears, numGears - 1);
            }
        }

        return 0;
    }

    private void OnGearSet(int gear)
    {
        int correctGear = CorrectGear(gear);
        currentGear.SetValueWithoutTriggeringEvent(correctGear);

        if (GearCount != 0)
            currentSpeed.Value = speeds[Mathf.Abs(correctGear)];
    }

    private void OnGearSwitch(int direction)
    {
        if (canSwitchGear)
        {
            if (downMeansStop && direction * currentGear < 0)
                currentGear.Value = 0;
            else
                currentGear.Value = CorrectGear(currentGear + direction);
        }
    }
}
