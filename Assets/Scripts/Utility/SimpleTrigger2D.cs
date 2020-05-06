using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SimpleTrigger2D : ValueChangeEventsBehaviour
{
    public ValueChangeEvent IsTriggered = ValueChangeEvent.New<bool>();
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
        IsTriggered.Set(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TriggeredCounter--;
        if (GettriggeredCounter() == 0) IsTriggered.Set(false);
    }
}
