using UnityEngine;

public class Poscillator : MonoBehaviour
{
    public Vector2 amplitude;
    public Vector2 frequency;

    private Vector3 startPosition;
    private float startTime;
    private Rigidbody2D rb2D;

    private void Start()
    {
        startPosition = transform.localPosition;
        startTime = Time.fixedTime;
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float time = Time.fixedTime - startTime;

        Vector3 offPos = new Vector3(
            amplitude.x * Mathf.Sin(time * frequency.x * (2 * Mathf.PI)),
            amplitude.y * Mathf.Sin(time * frequency.y * (2 * Mathf.PI)),
            0f);

        Vector2 moveToPosition = startPosition + offPos;

        if (rb2D == null)
            transform.localPosition = moveToPosition;
        else
        {
            rb2D.velocity = (moveToPosition - rb2D.position) / Time.fixedDeltaTime;
        }
    }
}
