using UnityEngine;
using UnityEngine.Events;

public class ValueEvent<T> : UnityEvent<T>, IEventGeneric
{
    private UnityEvent triggerEvent;

    public ValueEvent()
    {
        triggerEvent = new UnityEvent();
    }

    public void AddListener(UnityAction listener)
    {
        triggerEvent.AddListener(listener);
    }

    public void AddListener<t>(UnityAction<t> listener)
    {      
        if (listener is UnityAction<T>) base.AddListener(listener as UnityAction<T>);
        else Debug.LogError("Event/Listener type mismatch: " + typeof(T) + "/" + listener.GetType());
    }
    
    public void Invoke()
    {
        triggerEvent.Invoke();
    }

    public void Invoke(object value)
    {
        base.Invoke((T)value);
        //if (value is T) base.Invoke((T)value);
        //else Debug.LogError("Event/Listener type mismatch: " + typeof(T) + "/" + value.GetType());
    }

    public new void RemoveAllListeners()
    {
        triggerEvent.RemoveAllListeners();
        base.RemoveAllListeners();
    }

    public void RemoveListener(UnityAction listener)
    {
        triggerEvent.RemoveListener(listener);
    }

    public void RemoveListener<t>(UnityAction<t> listener)
    {
        if (listener is UnityAction<T>) base.RemoveListener(listener as UnityAction<T>);
        else Debug.LogError("Event/Listener type mismatch: " + typeof(T) + "/" + listener.GetType());
    }
}
