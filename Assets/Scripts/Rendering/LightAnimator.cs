using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightAnimator : MonoBehaviour
{
    public float frequency;
    public float phase;
    public float intensityAmplitude;
    public float rangeAmplitude;

    private Light thisLight;
    private float startIntensity;
    private float startRange;

    private void Awake()
    {
        thisLight = GetComponent<Light>();
        startIntensity = thisLight.intensity;
        startRange = thisLight.range;
    }

    void Update()
    {
        float t = Time.time;
        thisLight.intensity = startIntensity + intensityAmplitude * Mathf.Cos((phase + t * frequency) * 2f * Mathf.PI);
        thisLight.range = startRange + rangeAmplitude * Mathf.Cos((phase + t * frequency) * 2f * Mathf.PI);
    }
}
