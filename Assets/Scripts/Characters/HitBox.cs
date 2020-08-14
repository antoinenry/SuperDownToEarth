using UnityEngine;

public class HitBox : MonoBehaviour
{
    [System.Serializable]
    public struct Damage { public string tag; public int points; }
    
    public string[] tags;
    
    public Trigger hit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(string t in tags)
        {
            if (collision.CompareTag(t))
            {
                hit.Trigger();
                break;
            }
        }
    }
}
