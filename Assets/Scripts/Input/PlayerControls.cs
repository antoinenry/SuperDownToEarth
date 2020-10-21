using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public IntChangeEvent axisInput;
    public Trigger action1Input;
    public Trigger action2Input;

    public void ResetControls()
    {
        axisInput.Value = 0;
    }
}
