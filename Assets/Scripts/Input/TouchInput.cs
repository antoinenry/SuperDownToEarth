using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput
{
    public float duration { get; private set; }
    public List<Vector2> positions { get; private set; }
    public bool hold { get; private set; }

    public TouchInput()
    {
        duration = 0f;
        positions = new List<Vector2>();
        hold = false;
    }

    public void Clear()
    {
        duration = 0f;
        positions.Clear();
        hold = false;
    }

    public void Start(Vector2 position)
    {
        duration = 0f;
        positions.Add(position);
        hold = true;
    }

    public void Stay(float deltaTime)
    {
        duration += deltaTime;
        hold = true;
    }

    public void Move(float deltaTime, Vector2 position)
    {
        duration += deltaTime;
        positions.Add(position);
        hold = true;
    }

    public void End()
    {
        hold = false;
    }
}
