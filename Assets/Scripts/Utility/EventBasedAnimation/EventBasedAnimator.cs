using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EventBasedAnimator : MonoBehaviour
{
    public IValueChangeEvent vceTest;

    [System.Serializable]
    public struct EventAnimationConfiguration
    {
        public string animatorParameterName;
        public Component eventHubElement;
        public string eventParameterName;
    }

    [SerializeField] private EventsHub animatedHub = null;
    [SerializeField] [HideInInspector] private EventAnimationConfiguration[] animationConfigurations;
    public int NumAnimationConfigurations { get => animationConfigurations == null ? 0 : animationConfigurations.Length; }

    private Animator animator;
    private List<EventAnimation> eventAnimations;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        eventAnimations = new List<EventAnimation>();
    }

    private void Start()
    {
        EnableAnimations();
    }

    private void OnDestroy()
    {
        DisableAnimations();
    }

    protected void EnableAnimations()
    {
        if (animatedHub == null) return;

        foreach (EventAnimationConfiguration conf in animationConfigurations)
        {
            if(conf.eventHubElement is IEventsHubElement)
            {
                IEventsHubElement hubElement = (conf.eventHubElement as IEventsHubElement);
                int vceIndex = hubElement.GetValueChangeEventIndex(conf.eventParameterName);
                bool get = hubElement.GetValueChangeEvent(vceIndex, out IValueChangeEvent vce);

                if (get)
                    eventAnimations.Add(EventAnimation.NewEventAnimation(animator, vce, conf.animatorParameterName));
            }
        }
    }

    protected void DisableAnimations()
    {
        eventAnimations.Clear();
    }

    public EventsHub GetAnimatedObject()
    {
        return animatedHub;
    }
    
    public EventAnimationConfiguration GetAnimationConfiguration(int index)
    {
        if (animationConfigurations == null || index < 0 || index >= animationConfigurations.Length)
            return new EventAnimationConfiguration();
        else
            return animationConfigurations[index];
    }

    public bool SetAnimationConfiguration(int index, EventAnimationConfiguration conf)
    {
        if (animationConfigurations == null || index < 0 || index >= animationConfigurations.Length)
            return false;

        animationConfigurations[index] = conf;
        return true;
    }

    public int AddAnimationconfiguration()
    {
        List<EventAnimationConfiguration> confList = new List<EventAnimationConfiguration>(animationConfigurations);
        confList.Add(new EventAnimationConfiguration());
        animationConfigurations = confList.ToArray();

        return this.animationConfigurations.Length;
    }

    public int RemoveAnimationConfiguration(int atIndex)
    {
        if (this.animationConfigurations == null || atIndex < 0 || atIndex >= this.animationConfigurations.Length)
            return this.animationConfigurations.Length;

        List<EventAnimationConfiguration> confList = new List<EventAnimationConfiguration>(animationConfigurations);
        confList.RemoveAt(atIndex);
        animationConfigurations = confList.ToArray();

        return this.animationConfigurations.Length;
    }
}
