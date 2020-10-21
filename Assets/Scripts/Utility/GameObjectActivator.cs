using UnityEngine;

public class GameObjectActivator : MonoBehaviour
{
    public GameObject[] targets;

    public BoolChangeEvent active;
    public bool invertBool;

    private void Start()
    {
        if (active != null)
            active.AddValueListener<bool>(OnActivate);
    }

    private void OnDestroy()
    {
        active.RemoveValueListener<bool>(OnActivate);
    }

    private void OnActivate(bool setActive)
    {
        if (targets != null)
        {
            foreach(GameObject target in targets)
            {
                if (target != null)
                    target.SetActive(invertBool ? !setActive : setActive);
            }
        }
    }
}
