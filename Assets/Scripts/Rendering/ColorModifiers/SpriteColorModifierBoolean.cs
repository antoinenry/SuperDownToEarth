using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class SpriteColorModifierBoolean : ColorModifierBoolean
{
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();    
    }

    protected override Color GetCurrentColor()
    {
        return sprite.color;
    }

    protected override void SetCurrentColor(Color toColor)
    {
        sprite.color = toColor;
    }
}
