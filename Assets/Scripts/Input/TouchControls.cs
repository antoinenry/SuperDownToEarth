using System.Collections.Generic;
using UnityEngine;

public class TouchControls : MonoBehaviour
{
    public int maxTouch = 2;
    public float tapDuration = .1f;
    
    public Vector2[] Taps { get; private set; }
    public Vector2[] Holds { get; private set; }

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
        List<Vector2> taps = new List<Vector2>();
        List<Vector2> holds = new List<Vector2>();

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
                if (t.hold == false && t.duration <= tapDuration)
                    taps.Add(t.positions[0]);
                else if (t.hold == true && t.duration > tapDuration)
                    holds.Add(t.positions[t.positions.Count - 1]);
            }            
        }

        Taps = taps.ToArray();
        Holds = holds.ToArray();
    }
}
