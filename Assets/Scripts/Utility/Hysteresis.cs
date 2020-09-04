using System;
using UnityEngine;

[Serializable]
public class Hysteresis
{
    [Range(0f, 1f)] public float inferior = 0f;
    [Range(0f, 1f)] public float superior = 1f;

    private float input;
    public int Output { get; private set; }

    public float Input
    {
        set
        {
            input = value;

            if (Output == 0)
            {
                if (input >= superior) Output = 1;
            }
            else
            {
                if (input <= inferior) Output = 0;
            }
        }
    }

}
