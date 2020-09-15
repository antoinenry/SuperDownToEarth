using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageColorModifierBoolean : ColorModifierBoolean
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    protected override Color GetCurrentColor()
    {
        return image.color;
    }

    protected override void SetCurrentColor(Color toColor)
    {
        image.color = toColor;
    }
}
