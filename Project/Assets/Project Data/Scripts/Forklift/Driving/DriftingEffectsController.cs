using UnityEngine;

/// <summary>
/// Controls how drifting particles are displayed. These consist of a <see cref="bubbles"/> for the large volume of effects, <see cref="sparks"/> which are long trails, and <see cref="smokes"/> of the tyre.
/// Each effect is referenced in an array; index 0 refers to the left tyre and index 1 refers to the right tyre.
/// Generally, this class is exposing the minimum relevant <see cref="ParticleSystem"/> required for the system to work.
/// </summary>
public class DriftingEffectsController : MonoBehaviour
{
    // Generally shouldn't need to be accessed by other classes
    [Tooltip("The 0 index refers to the left tyre. The 1 index refers to the right tyre.")]
    public ParticleSystem[] bubbles, smokes, sparks, particles;

    // Purely to know how many groups of systems there are
    [SerializeField]
    GameObject[] mainObject;

    [GradientUsage(true), Tooltip("Gradient for the colour of the particle at a given tier.")]
    public Gradient boostGradient0, boostGradient1, boostGradient2, boostGradient3;

    // Cache colours to use during runtime
    ParticleSystem.MinMaxGradient[] tierGradients;
    bool playing;
    

    private void Awake()
    {
        playing = false;
        tierGradients = new ParticleSystem.MinMaxGradient[4];
        tierGradients[0] = new ParticleSystem.MinMaxGradient(boostGradient0);
        tierGradients[1] = new ParticleSystem.MinMaxGradient(boostGradient1);
        tierGradients[2] = new ParticleSystem.MinMaxGradient(boostGradient2);
        tierGradients[3] = new ParticleSystem.MinMaxGradient(boostGradient3);
    }

    /// <summary>
    /// Toggle emission for all particle systems.
    /// </summary>
    public void Emit(bool enabled)
    {
        ParticleSystem.EmissionModule emission;
        for (int i = 0; i < mainObject.Length; ++i)
        {
            emission = bubbles[i].emission; emission.enabled = enabled;
            emission = smokes[i].emission; emission.enabled = enabled;
            emission = sparks[i].emission; emission.enabled = enabled;
        }
    }

    /// <summary>
    /// Edit the visualisation of the particle corresponding to its tier.
    /// </summary>
    public void SetEffectTier(int tier = 0)
    {
        tier = Mathf.Clamp(tier, 0, 3);
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime;
        for (int i = 0; i < mainObject.Length; ++i)
        {
            colorOverLifetime = bubbles[i].colorOverLifetime;
            colorOverLifetime.color = tierGradients[tier];

            colorOverLifetime = sparks[i].colorOverLifetime;
            colorOverLifetime.color = tierGradients[tier];

            colorOverLifetime = particles[i].colorOverLifetime;
            colorOverLifetime.color = tierGradients[tier];
        }
    }

    /// <summary>
    /// Start all particle systems.
    /// </summary>
    public void Play()
    {
        if (playing) return;
        playing = true;

        for (int i = 0; i < mainObject.Length; ++i)
        {
            bubbles[i].Play();
            sparks[i].Play();
            smokes[i].Play();
            particles[i].Play();
        }
    }

    /// <summary>
    /// Stop all particle systems
    /// </summary>
    public void Stop()
    {
        playing = false;
        for (int i = 0; i < mainObject.Length; ++i)
        {
            bubbles[i].Stop();
            sparks[i].Stop();
            smokes[i].Stop();
            particles[i].Stop();
        }
    }

}
