using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EventParticleSystem : MonoBehaviour
{
    public Trigger play;
    public Trigger stop;
    public BoolChangeEvent playing;

    private ParticleSystem particles;
    private bool ignoreEvents;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        play.AddTriggerListener(OnPlay);
        stop.AddTriggerListener(OnStop);
        playing.AddValueListener<bool>(OnSetPlaying);
    }

    private void OnDisable()
    {
        play.RemoveTriggerListener(OnPlay);
        stop.RemoveTriggerListener(OnStop);
        playing.RemoveValueListener<bool>(OnSetPlaying);
    }

    private void OnPlay()
    {
        if (ignoreEvents) return;
        ignoreEvents = true;
        playing.Value = true;
        ignoreEvents = false;

        particles.Play();
    }

    private void OnStop()
    {
        if (ignoreEvents) return;
        ignoreEvents = true;
        playing.Value = false;
        ignoreEvents = false;

        particles.Stop();
    }

    private void OnSetPlaying(bool setToPlay)
    {
        if (ignoreEvents) return;
        if (setToPlay) play.Trigger();
        else stop.Trigger();
    }
}
