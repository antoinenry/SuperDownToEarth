using UnityEngine;

public class BoolComparator : MonoBehaviour
{
    public enum Operation { AEqualsB, ANotEqualsB, AAndB, AOrB, ANorB }

    public Operation comparaison;
    public float delay;

    public BoolChangeEvent inputA;
    public BoolChangeEvent inputB;
    public BoolChangeEvent output;

    public bool ComparaisonResult
    {
        get
        {
            switch (comparaison)
            {
                case Operation.AEqualsB:
                    return (bool)inputA.Value == (bool)inputB.Value;

                case Operation.ANotEqualsB:
                    return (bool)inputA.Value != (bool)inputB.Value;

                case Operation.AAndB:
                    return (bool)inputA.Value && (bool)inputB.Value;

                case Operation.AOrB:
                    return (bool)inputA.Value || (bool)inputB.Value;

                case Operation.ANorB:
                    return (bool)inputA.Value ^ (bool)inputB.Value;
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
