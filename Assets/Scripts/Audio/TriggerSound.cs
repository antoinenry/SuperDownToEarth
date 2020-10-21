using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TriggerSound : MonoBehaviour
{
    public AudioClip clip;
    [Range(0f, 1f)] public float randomizePitch;
    public Trigger play;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        play.AddTriggerListener(OnPlay);
    }

    private void OnDisable()
    {
        play.RemoveTriggerListener(OnPlay);
    }

    private void OnPlay()
    {
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.pitch = randomizePitch == 0f ? 1f : 1f + Random.Range(-randomizePitch, randomizePitch);
        audioSource.Play();
    }
}
