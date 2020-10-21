using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public BoolChangeEvent paused;

    private PausableGameObject[] pausables;

    private void Awake()
    {
        pausables = FindObjectsOfType<PausableGameObject>();
    }

    private void OnEnable()
    {
        foreach (PausableGameObject p in pausables)
            paused.AddValueListener<bool>(p.PauseComponents);
    }

    private void OnDisable()
    {
        foreach (PausableGameObject p in pausables)
            paused.RemoveValueListener<bool>(p.PauseComponents);
    }
}
