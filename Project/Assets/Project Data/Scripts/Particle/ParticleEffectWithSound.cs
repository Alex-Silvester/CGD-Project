using UnityEngine;
using UnityEngine.Audio;

public class ParticleEffectWithSound : ParticleEffect
{
    public AudioSource soundSource;
    public AudioMixerGroup mixerGroup;
    public float pitchRange = 0.05f;

    private void Awake()
    {
        if (soundSource == null) soundSource = GetComponent<AudioSource>();
        if (mixerGroup == null) Debug.LogWarning("No mixer group set for particle effect with sound");
        else soundSource.outputAudioMixerGroup = mixerGroup;
    }

    public override void Play()
    {
        soundSource.pitch *= Random.Range(-pitchRange, pitchRange) + 1;
        soundSource.Play();
        base.Play();
    }
}
