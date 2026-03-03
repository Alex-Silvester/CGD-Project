using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// You can add 'using static CrateExtensions' to use these things in any other class

/// <summary>
///  Collection of various crate extensions (structs, enums, functions, etc.)
/// </summary>
public static class CrateExtensions
{
    // Struct defining the requirements for spawning crates
    [Serializable]
    public class SpawnRequirements
    {
        [Tooltip("This must be the game object whose child transforms are used as spawn points.")]
        public Transform parentTransform;
        public CrateTag tag;
        public int spawnCount;
        public int crateScore;
        public DamageBehaviour damageBehaviour;

        /// <summary>
        /// Map of spawn points and the crate they are parented to
        /// </summary>
        public Dictionary<Transform, ICollectable> instances;

        /// <summary>
        /// Number of crates this requirement has spawned (i.e. instances which aren't null)
        /// </summary>
        public int Spawned => instances.Values.Count(item => (UnityEngine.Object)item != null);
        // Ugly lil cast since apparently the C# interpretation of the object and the Unity interpretation
        // of the object are two seperate things. 
        // Also this whole system might have a memory leak LOL
    }

    // Damage behaviour information for calculating crate damage
    [Serializable]
    public struct DamageBehaviour
    {
        [Tooltip("Collision velocity threshold for the crate to take damage. Higher values means the crate's velocity must be higher when colliding for damage.")]
        public float collisionVelocityForCrateDamage;

        [Tooltip("Damage multiplier, applied to the collision velocity above the collisionVelocityForCrateDamage threshold.")]
        public float damageCoefficient;

        [Tooltip("Maximum damage the crate can take as a numerical value."), Min(0f)]
        public float maximumScoreLossValue;

        [Tooltip("Maximum damage the crate can take as a percentage of the crate's maximum score."), Range(0f, 1f)]
        public float maximumScoreLossPercentage;

    }

    // A quota that the players must complete
    [Serializable]
    public struct ScheduleQuota
    {
        public CrateTag requiredTag;
        public float requiredScore;
        [Min(0f)]
        public float timeLimit;
    }

    // Struct defining a spawn node; a transform for where to spawn and a tag for its spawned object
    // This class also uses the transform's children as points
    [Serializable]
    public struct SpawnNode
    {
        public Transform transform;
        public CrateTag tag;
    }

    // Types of tags a crate can have. Add more to the enum if you want.
    // This can be referenced by calling CrateObject.CrateTag. 
    public enum CrateTag { Red, Green, Blue}

    // Gets a random crate tag
    public static CrateTag GetRandomCrateTag()
    {
        int length = Enum.GetNames(typeof(CrateTag)).Length;
        return (CrateTag)UnityEngine.Random.Range(0, length);
    }

    // Returns a colour for a given colour-named tag
    public static Color GetColourFromTag(this CrateTag tag)
    {
        return tag switch
        {
            CrateTag.Red => Color.red,
            CrateTag.Green => Color.green,
            CrateTag.Blue => Color.blue,
            _ => Color.white,
        };
    }

    // Randomise spawnable transforms (Fisher-Yates shuffle I found on stack overflow)
    // Partition list from 0 to pointer to end -> Select random element -> swap with pointer element -> decrement pointer
    public static void ShuffleList<T>(List<T> list)
    {
        var rnd = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

}
