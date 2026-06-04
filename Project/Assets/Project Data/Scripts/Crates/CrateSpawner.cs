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
    List<SpawnRequirements> spawnRequirements = new List<SpawnRequirements>();

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

            // Spawn more crates until we've reached the max spawn count or spawned at all valid points
            int j = 0,
                k = Mathf.Clamp(req.spawnCount,0 , validPoints.Count);
            for (int i = req.Spawned; i < k;  i++)
            {
                SpawnCrate(validPoints[j], req);
                j++;
            }
        }
    }

    // Spawns a crate and set its data based on its requirement. Instantiate within spawnedObjects
    void SpawnCrate(in Transform point, in SpawnRequirements requirement) 
        => requirement.instances[point] = CrateObject.Instantiate(cratePrefab, point, requirement.tag, requirement.damageBehaviour, requirement.crateScore);

    // Randomise spawnable transforms (Fisher-Yates shuffle I found on stack overflow)
    // Partition list from 0 to pointer to end -> Select random element -> swap with pointer element -> decrement pointer
    static void ShuffleList<T>(List<T> list)
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

    // UTILITY FUNCTIONS
#if UNITY_EDITOR
    [Space, Header("Editor utilities")]

    [SerializeField, Tooltip("Scale for the gizmo of a spawn point box. " +
        "As the size is a ratio between this value and their score, the value of this property determines the score for a box to be drawn with a 1x1x1 size.")]
    float spawnPointGizmoScale = 50f;

    [SerializeField, Tooltip(
        "Draw coloured cubes for each of the spawn points. " +
        "This will only work if drawGizmos is initially set to true. " +
        "if drawGizmosOnSelected is true, the boxes are only shown if this object is selected.")]
    bool drawGizmos, drawGizmosOnSelected;

    private void OnDrawGizmosSelected()
    {
        if (drawGizmos && drawGizmosOnSelected) DrawSpawnPointGizmos();
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos && !drawGizmosOnSelected) DrawSpawnPointGizmos();
    }

    // Draws the spawner locations and what colour they are for. Size is also based on the score 
    private void DrawSpawnPointGizmos()
    {
        foreach (var req in spawnRequirements)
        {
            foreach (Transform t in req.parentTransform.GetComponentsInChildren<Transform>().Skip(1).ToArray())
            {
                Gizmos.color = req.tag.GetColourFromTag();
                Gizmos.DrawCube(t.position, new Vector3(1f, 1f, 1f) * (req.crateScore * (1/ spawnPointGizmoScale)));
            }
        }
    }

    // Apply a default damage behaviour
    private void ResetAllDamageBehaviours()
    {
        DamageBehaviour def = new()
        {
            collisionVelocityForCrateDamage = 10f,
            damageCoefficient = 0.4f,
            maximumScoreLossValue = 10f,
            maximumScoreLossPercentage = 0f
        };
        foreach(var req in spawnRequirements)
        {
            req.damageBehaviour = def;
        }
    }
#endif

}
