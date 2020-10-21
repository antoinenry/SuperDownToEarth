using System;
using UnityEngine.Events;

public interface IEventGeneric
{
    void Invoke();
    void Invoke(object value);

    void AddListener(UnityAction listener);
    void AddListener<t>(UnityAction<t> listener);

    void RemoveListener(UnityAction listener);
    void RemoveListener<t>(UnityAction<t> listener);
    
    void RemoveAllListeners();
}