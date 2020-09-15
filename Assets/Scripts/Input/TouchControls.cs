using System.Collections.Generic;
using UnityEngine;

public class TouchControls : MonoBehaviour
{
    public int maxTouch = 2;
    public float minHoldDuration = .1f;
    public float maxTapDuration = .3f;
    
    public Vector2[] Taps { get; private set; }
    public Vector2[] Holds { get; private set; }

    public BoolChangeEvent touchRight;
    public BoolChangeEvent touchLeft;

    private List<TouchInput> touchInputs;

    private void Awake()
    {
        Taps = new Vector2[0];
        Holds = new Vector2[0];

        touchInputs = new List<TouchInput>(maxTouch);
        for (int i = 0; i < maxTouch; i++) touchInputs.Add(new TouchInput());
    }

    private void Update()
    {
        FetchTouchInputs();
    }

    private void FetchTouchInputs()
    {
        List<Vector2> taps = new List<Vector2>();
        List<Vector2> holds = new List<Vector2>();
        bool holdingRight = false;
        bool holdingLeft = false;

        int touchCount = Input.touchCount;

        for (int i = 0; i < maxTouch; i++)
        {
            if (i < touchCount)
            {
                Touch touch = Input.GetTouch(i);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchInputs[i].Start(touch.position);
                        break;

                    case TouchPhase.Stationary:
                        touchInputs[i].Stay(touch.deltaTime);
                        break;

                    case TouchPhase.Moved:
                        touchInputs[i].Move(touch.deltaTime, touch.position);
                        break;

                    case TouchPhase.Ended:
                        touchInputs[i].End();
                        break;
                }
            }
            else
                touchInputs[i].Clear();

            TouchInput t = touchInputs[i];
            if (t.positions.Count >= 1)
            {
                if (t.hold == true && t.duration >= minHoldDuration)
                {
                    Vector2 holdPosition = t.positions[t.positions.Count - 1];
                    holds.Add(holdPosition);
                    if (holdPosition.x > Screen.width / 2f)
                        holdingRight = true;
                    else
                        holdingLeft = true;
                }

                if (t.hold == false && t.duration <= maxTapDuration)
                {
                    taps.Add(t.positions[0]);
                }
            }
        }

        Taps = taps.ToArray();
        Holds = holds.ToArray();
        touchRight.Value = holdingRight;
        touchLeft.Value = holdingLeft;
    }
}
