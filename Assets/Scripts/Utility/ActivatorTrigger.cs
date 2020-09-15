using UnityEngine;

public class ActivatorTrigger : MonoBehaviour
{
    public GameObject target;
    public BoolChangeEvent activateOnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activateOnTriggerEnter && target != null)
        {
            target.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (activateOnTriggerEnter && target != null)
        {
            target.SetActive(false);
        }
    }
}
