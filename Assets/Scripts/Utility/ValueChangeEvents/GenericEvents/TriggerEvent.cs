using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : UnityEvent, IEventGeneric
{
    public void AddListener<t>(UnityAction<t> listener)
    {
        throw new System.NotImplementedException();
    }

    public void Invoke(object value)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveListener<t>(UnityAction<t> listener)
    {
        throw new System.NotImplementedException();
    }
}
