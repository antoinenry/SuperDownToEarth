using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
    public Material material;
    public Vector2 scaleAmplitude;
    public Vector2 scaleFrequency;
    public Vector2 offsetAmplitude;
    public Vector2 offsetFrequency;

    private Vector2 startScale;
    private Vector2 startOffset;

    private void Start()
    {
        startScale = material.mainTextureScale;
        startOffset = material.mainTextureOffset;
    }

    private void Update()
    {
        float time = Time.time;

        material.mainTextureScale = startScale + new Vector2(scaleAmplitude.x * Mathf.Cos(time * scaleFrequency.x), scaleAmplitude.y * Mathf.Cos(time * scaleFrequency.y));
        material.mainTextureOffset = startOffset + new Vector2(offsetAmplitude.x * Mathf.Cos(time * offsetFrequency.x), offsetAmplitude.y * Mathf.Cos(time * offsetFrequency.y));
    }

    private void OnDisable()
    {
        material.mainTextureScale = startScale;
        material.mainTextureOffset = startOffset;
    }
}
