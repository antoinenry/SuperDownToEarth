using System.Collections;
using UnityEngine;

public abstract class ColorModifierBoolean : MonoBehaviour
{
    public Color modifiedColor = Color.white;
    [Min(0f)] public float fadeTime;

    public BoolChangeEvent modifyColor;

    private Color defaultColor;

    private void Start()
    {
        defaultColor = GetCurrentColor();
    }

    private void OnEnable()
    {
        modifyColor.AddValueListener<bool>(OnModifyColor);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        modifyColor.RemoveValueListener<bool>(OnModifyColor);
    }

    protected abstract Color GetCurrentColor();
    protected abstract void SetCurrentColor(Color toColor);

    private void OnModifyColor(bool mod)
    {
        Color fromColor = GetCurrentColor();
        Color toColor = mod ? modifiedColor : defaultColor;

        if (fadeTime <= 0f)
            SetCurrentColor(toColor);
        else
            StartCoroutine(FadeCoroutine(fromColor, toColor));
    }

    private IEnumerator FadeCoroutine(Color fromColor, Color toColor)
    {
        float fadeTimer = 0f;
        Color deltaColor = toColor - fromColor;

        while (fadeTimer < fadeTime)
        {
            SetCurrentColor(fromColor + deltaColor * (fadeTimer / fadeTime));

            yield return new WaitForFixedUpdate();
            fadeTimer += Time.fixedDeltaTime;
        }

        SetCurrentColor(toColor);
    }
}
