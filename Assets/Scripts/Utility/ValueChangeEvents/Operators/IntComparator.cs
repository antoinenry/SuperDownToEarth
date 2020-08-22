using UnityEngine;

public class IntComparator : MonoBehaviour
{
    public enum Operation { AEqualToB, ANotEqualToB, AGreaterOrEqualThanB, ALessOrEqualThanB }

    public Operation comparaison;
    public float delay;

    public IntChangeEvent inputA;
    public IntChangeEvent inputB;
    public BoolChangeEvent output;

    public bool ComparaisonResult
    {
        get
        {
            switch (comparaison)
            {
                case Operation.AEqualToB:
                    return (int)inputA.Value == (int)inputB.Value;

                case Operation.AGreaterOrEqualThanB:
                    return (int)inputA.Value >= (int)inputB.Value;

                case Operation.ALessOrEqualThanB:
                    return (int)inputA.Value <= (int)inputB.Value;

                case Operation.ANotEqualToB:
                    return (int)inputA.Value != (int)inputB.Value;
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
