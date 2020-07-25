using UnityEngine;

public class DamageStatus : MonoBehaviour
{
    public string[] damagingTags;

    public Trigger death;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(string tag in damagingTags)
        {
            if (collision.CompareTag(tag))
            {
                GetDamaged();
                break;
            }
        }
    }

    public void GetDamaged()
    {
        death.Trigger();
    }
}
