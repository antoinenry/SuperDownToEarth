using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonToggle : MonoBehaviour, IPointerDownHandler
{
    public Sprite onSprite;
    public Sprite offSprite;
    public BoolChangeEvent pressed;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed.Value = !pressed;
        button.image.sprite = pressed ? onSprite : offSprite;
    }
}
