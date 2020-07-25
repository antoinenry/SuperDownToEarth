using System;
using UnityEngine;

public class EmoteSystem : MonoBehaviour
{
    public enum EmoteType { None, Particles }

    [Serializable]
    public struct Emote
    {
        public EmoteType type;
        public int spriteIndexMin;
        public int spriteIndexMax;
    }

    public Emote[] emotes;
    public IntChangeEvent currentEmoteIndex;

    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        if (currentEmoteIndex == null) currentEmoteIndex = new IntChangeEvent();
        currentEmoteIndex.AddTriggerListener(OnChangeEmote, true);
    }

    private void OnDestroy()
    {
        currentEmoteIndex.RemoveTriggerListener(OnChangeEmote);
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
}
