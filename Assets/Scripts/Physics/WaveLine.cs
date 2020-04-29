using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WaveLine : MonoBehaviour
{
    [Min(0f)] public float waveWidth;
    [Min(0f)] public float waveAmplitude;
    [Min(0f)] public float waveFrequency;
    [Min(1)] public int waveCount;

    private LineRenderer line;
    
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(Application.isPlaying)
        {
            float timeOff = (Time.time * waveFrequency / (float)waveCount) % 1f;
            SetWaves(timeOff);
        }
    }

    private void OnGUI()
    {
        SetWaves();
    }

    private void SetWaves(float timeOffset = 0f)
    {
        List<Keyframe> keys = new List<Keyframe>();
        float startWidth = waveWidth + waveAmplitude * Mathf.Cos(timeOffset * 2 * Mathf.PI);
        float topWidth = waveWidth + waveAmplitude;
        float bottomWidth = waveWidth - waveAmplitude;

        keys.Add(new Keyframe(0f, startWidth));

        float step = 1f / (float)waveCount;
        for (float t = timeOffset; t < timeOffset + 1f; t += step)
        {
            keys.Add(new Keyframe(t % 1f, topWidth));
            keys.Add(new Keyframe((t + step / 2f) % 1f, bottomWidth));
        }

        keys.Add(new Keyframe(1f, startWidth));

        line.widthCurve = new AnimationCurve(keys.ToArray());
    }
}
