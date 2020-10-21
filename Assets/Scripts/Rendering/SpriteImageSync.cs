using UnityEngine;
using UnityEngine.UI;

public class SpriteImageSync : MonoBehaviour
{
    public enum Way { SpriteToImage, ImageToSprite }

    public SpriteRenderer syncedSprite;
    public Image syncedImage;
    public Way syncWay;
    public bool syncSprite = true;
    public bool syncColor = true;
    
    void Update()
    {
        switch (syncWay)
        {
            case Way.SpriteToImage:
                if (syncSprite) syncedImage.sprite = syncedSprite.sprite;
                if (syncColor) syncedImage.color = syncedSprite.color;
                break;

            case Way.ImageToSprite:
                if (syncSprite) syncedSprite.sprite = syncedImage.sprite;
                if (syncColor) syncedSprite.color = syncedImage.color;
                break;
        }
    }
}
