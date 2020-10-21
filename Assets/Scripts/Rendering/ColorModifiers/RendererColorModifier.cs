using UnityEngine;

[ExecuteAlways]
public class RendererColorModifier : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public Light[] lightRenderers;
    public TrailRenderer[] trailRenderers;
    public ParticleSystem[] particleRenderers;

    public ColorChangeEvent currentColor;

    private void Reset()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        lightRenderers = GetComponentsInChildren<Light>();
        trailRenderers = GetComponentsInChildren<TrailRenderer>();
        particleRenderers = GetComponentsInChildren<ParticleSystem>();

        if (currentColor == null) currentColor = new ColorChangeEvent();

        if (spriteRenderers != null && spriteRenderers.Length > 0) currentColor.Value = spriteRenderers[0].color;
        else if (lightRenderers != null && lightRenderers.Length > 0) currentColor.Value = lightRenderers[0].color;
        else if (trailRenderers != null && trailRenderers.Length > 0) currentColor.Value = trailRenderers[0].startColor;
        else if (particleRenderers != null && particleRenderers.Length > 0) currentColor.Value = particleRenderers[0].main.startColor;
        else currentColor.Value = Color.white;
    }

    private void OnEnable()
    {
        currentColor.AddValueListener<Color>(OnColorChange);
    }

    private void OnDisable()
    {
        currentColor.RemoveValueListener<Color>(OnColorChange);
    }

    private void OnColorChange(Color newColor)
    {
        if (spriteRenderers != null)
            foreach (SpriteRenderer sr in spriteRenderers)
                sr.color = new Color(newColor.r, newColor.g, newColor.b, sr.color.a);

        if (lightRenderers != null)
            foreach (Light lr in lightRenderers)
                lr.color = new Color(newColor.r, newColor.g, newColor.b, lr.color.a);

        if (trailRenderers != null)
            foreach (TrailRenderer tr in trailRenderers)
            {
                Color trailColor = new Color(newColor.r, newColor.g, newColor.b, tr.startColor.a);
                tr.startColor = trailColor;
                tr.endColor = trailColor;
            }

        if (particleRenderers != null)
            foreach (ParticleSystem pr in particleRenderers)
            {
                ParticleSystem.MainModule mainModule = pr.main;
                Color particleColor = new Color(newColor.r, newColor.g, newColor.b, mainModule.startColor.color.a);
                mainModule.startColor = particleColor;
            }
    }

    public void SetColor(Color toColor)
    {
        currentColor.Value = toColor;
    }
}
