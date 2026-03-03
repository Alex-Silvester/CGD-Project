using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CrateExtensions;

/// <summary>
/// MonoBehaviour which handles spawning collectable crates.
/// </summary>
[RequireComponent(typeof(Timer))]
public class CrateSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cratePrefab;

    [SerializeField, Tooltip("Timeout event is assigned at runtime.")]
    Timer timer;

    [SerializeField, Tooltip("How many objects should be spawned for a given tag."), ContextMenuItem("Apply default damage behaviour", "ResetAllDamageBehaviours")]
    List<SpawnRequirements> spawnRequirements;

    private void OnValidate()
    {
        // Valid prefab
        if (cratePrefab == null || !cratePrefab.TryGetComponent(out ICollectable _))
        {
            Debug.LogWarning("Spawner does not have correct crate prefab.");
        }

        if (TryGetComponent(out timer))
        {
            timer.repeat = true;
            timer.autoStart = false;
        }
    }

    void Initialise()
    {
        // Initialise the instances map on each spawn requirement
        for (int i = 0; i < spawnRequirements.Count; i++) 
        {
            var dict = new Dictionary<Transform, ICollectable>();
            foreach (var t in spawnRequirements[i].parentTransform.GetComponentsInChildren<Transform>().Skip(1).ToArray())
            {
                dict.Add(t, null);
            }

            spawnRequirements[i].instances = dict;
        }
    }

    private void Awake()
    {
        Initialise();
    }

    private void OnEnable()
    {
        if (timer != null)
        {
            timer.timeout.AddListener(TrySpawnCrates);
        }
    }

    private void OnDisable()
    {
        if (timer != null)
        {
            timer.timeout.RemoveListener(TrySpawnCrates);
        }
    }

    public void StartSpawner() => timer.paused = false;
    public void StopSpawner() => timer.paused = true;

    // Attempts to spawn a crate at each point if its mapped GameObject is null
    void TrySpawnCrates()
    {
        // Loop through each requirement and spawn in as many crates are needed
        foreach (var req in spawnRequirements)
        {
            // Get a shuffled list of the spawn transforms which do not have any objects
            List<Transform> allPoints = req.instances.Keys.ToList();
            List<Transform> validPoints = allPoints.FindAll(item => (Object)req.instances[item] == null);
            ShuffleList(validPoints);

            // Spawn more crates until we've reached the max spawn count
            int j = 0;
            for (int i = req.Spawned; i < req.spawnCount;  i++)
            {
                SpawnCrate(validPoints[j], req);
                j++;
            }
        }
    }

    // Spawns a crate and set its data based on its requirement. Instantiate within spawnedObjects
    void SpawnCrate(in Transform point, in SpawnRequirements requirement) 
        => requirement.instances[point] = CrateObject.Instantiate(cratePrefab, point, requirement.tag, requirement.damageBehaviour, requirement.crateScore);

    // Draws the spawner locations and what colour they are for. Size is also based on the score 
    private void OnDrawGizmosSelected()
    {
        foreach (var req in spawnRequirements)
        {
            foreach (Transform t in req.parentTransform.GetComponentsInChildren<Transform>().Skip(1).ToArray())
            {
                Gizmos.color = req.tag.GetColourFromTag();
                Gizmos.DrawCube(t.position, new Vector3(1, 1, 1) * (req.crateScore / 75f));
            }
        }
    }

    // UTILITY FUNCTION
    private void ResetAllDamageBehaviours()
    {
        foreach(var req in spawnRequirements)
        {
            req.damageBehaviour = new()
            {
                collisionVelocityForCrateDamage = 10f,
                damageCoefficient = 0.4f,
                maximumScoreLossValue = 10f,
                maximumScoreLossPercentage = 0f
            };
        }
    }
}
