using System;
using UnityEngine;

public class EmoteSystem : MonoBehaviour
{
    /*
    public enum EmoteType { None, Particles }

    [Serializable]
    public struct Emote
    {
        public string name;
        public EmoteType type;
        public int spriteIndexMin;
        public int spriteIndexMax;
    }

    public Emote[] emotes;
    public StringChangeEvent currentEmote;

    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        currentEmote.AddTriggerListener(OnChangeEmote, true);
    }

    private void OnDestroy()
    {
        currentEmote.RemoveTriggerListener(OnChangeEmote);
    }

    private void OnChangeEmote()
    {
        if (emotes != null && currentEmoteIndex >= 0 && currentEmoteIndex < emotes.Length)
            CurrentEmote = emotes[currentEmoteIndex];
    }

    public Emote CurrentEmote
    {
        get
        {
            if (emotes != null && currentEmoteIndex >= 0 && currentEmoteIndex < emotes.Length)
            {
                return emotes[currentEmoteIndex];
            }
            else
            {
                Debug.LogError("Current emote index is out of range");
                return new Emote();
            }
        }
        set
        {
            if (particles != null)
            {
                if (value.type == EmoteType.Particles)
                {
                    ParticleSystem.TextureSheetAnimationModule sheetModule = particles.textureSheetAnimation;
                    float spriteCount = sheetModule.spriteCount;
                    sheetModule.startFrame = new ParticleSystem.MinMaxCurve(value.spriteIndexMin / spriteCount, value.spriteIndexMax / spriteCount);
                    particles.Play();
                }
                else
                {
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }            
        }
    }
    */
}
