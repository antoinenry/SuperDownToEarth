using UnityEngine;

public class FloatComparator : MonoBehaviour
{
    public enum Operation { AEqualToB, ANotEqualToB, AGreaterOrEqualThanB, ALessOrEqualThanB }

    public Operation comparaison;
    public float delay;

    public FloatChangeEvent inputA;
    public FloatChangeEvent inputB;
    public BoolChangeEvent output;

    public bool ComparaisonResult
    {
        get
        {
            switch (comparaison)
            {
                case Operation.AEqualToB:
                    return (float)inputA.Value == (float)inputB.Value;

                case Operation.AGreaterOrEqualThanB:
                    return (float)inputA.Value >= (float)inputB.Value;

                case Operation.ALessOrEqualThanB:
                    return (float)inputA.Value <= (float)inputB.Value;

                case Operation.ANotEqualToB:
                    return (float)inputA.Value != (float)inputB.Value;
            }

            return false;
        }
    }

    private void OnEnable()
    {
        inputA.AddTriggerListener(OnInputChange);
        inputB.AddTriggerListener(OnInputChange);
        SetOutput();
    }

    private void OnDisable()
    {
        inputA.RemoveTriggerListener(OnInputChange);
        inputB.RemoveTriggerListener(OnInputChange);
    }

    private void OnInputChange()
    {
        if (delay > 0f)
        {
            CancelInvoke();
            Invoke("SetOutput", delay);
        }
        else
            SetOutput();
    }
    
    private void SetOutput()
    {
        output.Value = ComparaisonResult;
    }
}
