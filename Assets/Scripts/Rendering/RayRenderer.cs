using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RayRenderer : MonoBehaviour
{
    public LayerMask stoppingLayer;
    public float minDistance = 1f;
    public float maxDistance = 10f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        Vector3 director = transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + director * minDistance, director, maxDistance, stoppingLayer);
        bool enableRay = false;

        if (hit.collider != null)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance > minDistance && distance < maxDistance)
            {
                SetRay(distance, director);
                enableRay = true;
            }
        }

        lineRenderer.enabled = enableRay;
    }

    private void SetRay(float distance, Vector3 director)
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, transform.position + minDistance * director);
        lineRenderer.SetPosition(1, transform.position + distance * director);
    }
}
