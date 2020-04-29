using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParticles : MonoBehaviour
{
    public string cameraTag = "MainCamera";
    public float parallaxScale = 1f;

    private Rigidbody2D cameraBody;

    private ParticleSystem starParticles;

    private void Awake()
    {
        starParticles = GetComponent<ParticleSystem>();
        GameObject camGO = GameObject.FindGameObjectWithTag(cameraTag);

        if (camGO != null)
        {
            cameraBody = camGO.GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {
        FakeParallax();
    }

    private void FakeParallax()
    {
        Vector3 cameraPosition = cameraBody.position;
        Vector3 parallaxedPosition = new Vector3(cameraPosition.x * parallaxScale, cameraPosition.y * parallaxScale, transform.position.z);

        transform.position = parallaxedPosition;

        ParticleSystem.ShapeModule particleShape = starParticles.shape;
        particleShape.position = new Vector3(cameraPosition.x - parallaxedPosition.x, cameraPosition.y - parallaxedPosition.y, 0f);
    }
}
