using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class InsideSwitch : MonoBehaviour
{
    public float fadeSpeed;
    public Material[] outsideMaterials;
    public Light[] outsideLights;

    [HideInInspector] public bool showInside;

    private float outsideAlpha;

    private void Update()
    {
        Fade(out bool isFading);

        if (isFading) RenderOpacity();
    }

    private void Fade(out bool isFading)
    {
        if (showInside && outsideAlpha > 0f)
        {
            isFading = true;
            outsideAlpha = Mathf.Lerp(outsideAlpha, 0f, Time.deltaTime * fadeSpeed);
        }
        else if (!showInside && outsideAlpha < 1f)
        {
            isFading = true;
            outsideAlpha = Mathf.Lerp(outsideAlpha, 1f, Time.deltaTime * fadeSpeed);
        }
        else
            isFading = false;
    }

    private void RenderOpacity()
    {
        if (outsideMaterials != null)
            foreach (Material m in outsideMaterials)
                m.color = new Color(m.color.r, m.color.g, m.color.b, outsideAlpha);

        if (outsideLights != null)
            foreach (Light l in outsideLights)
                l.intensity = outsideAlpha;
    }

    public void ShowInsideImmediate(bool show)
    {
        showInside = show;
        outsideAlpha = showInside ? 0f : 1f;
        RenderOpacity();
    }
}
