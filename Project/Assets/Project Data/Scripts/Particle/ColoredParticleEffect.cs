using UnityEngine;

// Implements a colour to override the particle effect's main colour.
// This affects both the vertex colour (color property on ParticleSystem.main) and the material colour property.
public class ColoredParticleEffect : ParticleEffect
{
    public Color color = Color.white;

    [ColorUsage(true, true)]
    public Color emissionColor = Color.white;

    public float emissionIntensity = 1f;

    public override void Play()
    {
        var main = ParticleSystem.main;
        main.startColor = color;
        var rend = GetComponent<ParticleSystemRenderer>();
        var material = rend.material;
        var trailMaterial = rend.trailMaterial;
        if (material != null)
        {
            material.SetColor("_BaseColor", color);
            material.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        }
        if (trailMaterial != null)
        {
            trailMaterial.SetColor("_BaseColor", color);
            trailMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        }
        base.Play();
    }
    
}