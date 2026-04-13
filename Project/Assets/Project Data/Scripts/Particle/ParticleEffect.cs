using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

/* I decided to make the particle effect system revolve around this abstract class since it seemed to be the most simple method of implementing  
 * a 'Particle System Library' that could host different kinds of particle effects.
 * The idea is that any particle system that has some special code to function would implement this class so that it could still be added in the library.
 * I didn't want there to be a lot of separate particle effect classes and instead thought it would be neater if they all could exist in the same space.
 */

// --- IMPORTANT ---
// To create a new type of particle effect, make a new C# code that inherits from ParticleEffect.
// Note that particle effects with their particle system's "play on awake" OR "looping" set to true will not be automatically managed by the system.
// This system is not intended for particle systems with "play on awake" set to true 

/// <summary>
/// Represents a particle system on a game object.
/// This class has the basic functions for handling the lifetime of the particle effect and its particle system.
/// Any more complex particle effect should be casted to a specified type.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public abstract class ParticleEffect : MonoBehaviour
{
    ParticleSystem ps;
    List<AudioSource> sounds = new();

    // When first getting the particle system it will fetch the component
    public ParticleSystem ParticleSystem 
    {
        get => GetPS();
        protected set => ps = value; 
    }

    public ParticleEffect AtPosition(Vector3 position)
    {
        transform.position = position;
        return this;
    }

    public ParticleEffect AtRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
        return this;
    }

    /// <summary>
    /// Add a sound to the particle effect.
    /// </summary>
    /// <param name="clip"> Sound clip to play. </param>
    /// <param name="mixer"> Mixer for audio output. </param>
    /// <param name="volume"> Volume of the clip. </param>
    /// <param name="pitch"> Pitch of the clip. </param>
    /// <param name="pitchRandomise"> The randomised pitch range the clip can play at. </param>
    /// <returns></returns>
    public ParticleEffect WithSound(AudioClip clip, AudioMixerGroup mixer = default, float volume = 1f, float pitch = 1f, Vector2 pitchRandomise = default)
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.transform.parent = transform;
        sounds.Add(source);
        float newPitch = Mathf.Clamp(pitchRandomise == default ? pitch : Random.Range(pitch - pitchRandomise.x, pitch + pitchRandomise.y), -3f, 3f);

        // Assign clip stuff
        source.clip = clip;
        source.loop = false;
        source.playOnAwake = false;
        source.outputAudioMixerGroup = (mixer == default) ? ParticleEffectLibrary.defaultMixer : mixer;
        source.volume = Mathf.Clamp(volume, 0f, 1f);
        source.pitch = newPitch;

        return this;
    }

    /// <summary>
    /// Calls <see cref="ParticleSystem.Play()"/> and destroy's this object once the effect stops.
    /// </summary>
    /// <remarks> When overriding this function, ensure that any logic or setup is done before calling base.Play() </remarks>
    public virtual void Play()
    {
        var main = ParticleSystem.main;
        if (!ParticleSystem.isPlaying) ParticleSystem.Play();
        if (sounds.Count > 0) foreach (var sound in sounds) sound.Play();
        if (!main.loop || main.playOnAwake) StartCoroutine(YieldForDestroy());
    }

    // Destroys this object when the particle system stops playing
    IEnumerator YieldForDestroy()
    {
        yield return new WaitWhile(() => ParticleSystem.isPlaying && IsSoundPlaying);
        Destroy(gameObject);
    }

    // If there are sounds, it will return true if any of them are playing
    bool IsSoundPlaying
    {
        get
        {
            if (sounds.Count == 0) return false;
            foreach (var sound in sounds)
            {
                if (sound.isPlaying) return true;
            }
            return false;
        }
    }

    ParticleSystem GetPS()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();
        return ps;
    }

}
