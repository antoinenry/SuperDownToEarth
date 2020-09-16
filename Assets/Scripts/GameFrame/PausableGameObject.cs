using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class PausableGameObject : MonoBehaviour
{
    public GameObject root;
    public Component[] pausableComponents;
    public BoolChangeEvent paused;

    private void Start()
    {
        if (root == null)
        {
            root = gameObject;
            GetAllPausableComponents(out pausableComponents);
        }
    }

    private void OnEnable()
    {
        if (paused == null) paused = new BoolChangeEvent();
        paused.AddValueListener<bool>(OnPause);
    }

    private void OnDisable()
    {
        paused.RemoveValueListener<bool>(OnPause);
    }

    public int GetAllPausableComponents(out Component[] pausables)
    {
        if (root == null)
        {
            pausables = new Component[0];
        }
        else
        {
            List<Component> allChildrenComponents = new List<Component>(root.GetComponentsInChildren<Component>(true));
            pausables = allChildrenComponents.FindAll(c => c is IPausable).ToArray();
        }

        return pausables.Length;
    }

    public void PauseComponents(bool pause)
    {
        paused.Value = pause;
    }

    private void OnPause(bool pause)
    {
        if (pausableComponents == null) return;

        foreach(Component c in pausableComponents)
        {
            if (c is IPausable)
            {
                IPausable pausable = c as IPausable;
                pausable.Pause(pause);
            }
        }
    }
}
