
using UnityEngine;

public class ToggleSound : MonoBehaviour
{
    public AudioClip clip;
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
        play.AddValueListener<bool>(OnPlay);
    }

    private void OnPlay(bool start)
    {
        if (start)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
