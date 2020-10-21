using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SimpleTrigger2D : MonoBehaviour
{
    public BoolChangeEvent IsTriggered;
    public int TriggeredCounter { get; private set; }
    
    public int GettriggeredCounter()
    {
        return TriggeredCounter;
    }

    private void SettriggeredCounter(int value)
    {
        TriggeredCounter = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggeredCounter++;
        IsTriggered.Value = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TriggeredCounter--;
        if (GettriggeredCounter() == 0) IsTriggered.Value = false;
    }
}
