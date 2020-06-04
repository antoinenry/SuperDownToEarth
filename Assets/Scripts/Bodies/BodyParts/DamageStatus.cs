using UnityEngine;
using UnityEngine.Events;

public class DamageStatus : BodyPart
{
    public string[] damagingTags;
    public UnityEvent OnDeath;

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
        OnDeath.Invoke();
    }
}
