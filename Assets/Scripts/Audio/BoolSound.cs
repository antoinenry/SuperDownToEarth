using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoolSound : MonoBehaviour
{
    public AudioClip trueClip;
    public AudioClip falseClip;
    public bool loopTrueClip;
    public bool loopFalseClip;
    public bool stopOnFalse;
    [Range(0f, 1f)] public float randomizePitch;

    public BoolChangeEvent play;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        play.AddValueListener<bool>(OnPlay);
    }

    private void OnDisable()
    {
        play.RemoveValueListener<bool>(OnPlay);
    }

    private void OnPlay(bool playValue)
    {
        if (playValue)
        {
            if (trueClip != null)
            {
                audioSource.clip = trueClip;
                audioSource.loop = loopTrueClip;
                audioSource.pitch = randomizePitch == 0f ? 1f : 1f + Random.Range(-randomizePitch, randomizePitch);
                audioSource.Play();
            }
        }
        else if (!playValue)
        {
            if (stopOnFalse) audioSource.Stop();
            if (falseClip != null)
            {
                audioSource.clip = falseClip;
                audioSource.loop = loopFalseClip;
                audioSource.pitch = randomizePitch == 0f ? 1f : 1f + Random.Range(-randomizePitch, randomizePitch);
                audioSource.Play();
            }
        }
    }
}
