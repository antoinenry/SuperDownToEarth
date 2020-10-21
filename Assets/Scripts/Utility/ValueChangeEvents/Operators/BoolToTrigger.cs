using UnityEngine;

public class BoolToTrigger : MonoBehaviour
{
    public bool triggerValue;
    public float delay;

    public BoolChangeEvent input;
    public Trigger output;

    private void OnEnable()
    {
        input.AddValueListener<bool>(OnInputChange);
    }

    private void OnDisable()
    {
        input.RemoveValueListener<bool>(OnInputChange);
    }

    private void OnInputChange(bool value)
    {
        if (value == triggerValue)
        {
            if (delay > 0f)
            {
                CancelInvoke();
                Invoke("TriggerOutput", delay);
            }
            else
                TriggerOutput();
        }
    }

    private void TriggerOutput()
    {
        output.Trigger();
    }
}
