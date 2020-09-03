using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RotaBody : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb2d.angularVelocity = speed;
    }
}
