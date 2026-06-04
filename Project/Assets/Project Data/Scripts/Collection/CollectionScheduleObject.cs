using System.Collections.Generic;
using UnityEngine;
using static CrateExtensions;
using System.Linq;

/// <summary>
/// Contains the schedule for how a collector should collect crates
/// </summary>
[CreateAssetMenu(fileName = "CollectionScheduleObject", menuName = "Scriptable Objects/CollectionScheduleObject")]
public class CollectionScheduleObject : ScriptableObject
{
    [SerializeField]
    ScheduleQuota[] collectionSchedule;

    [SerializeField, Tooltip("Whether this schedule object's tag requirements should be randomised.")]
    bool randomiseRequiredTag = false;

    [SerializeField, Min(1f)]
    float quotaBonusMultiplier = 1f;

    [SerializeField]
    float penaltyScoreMultiplier = 0.8f;

    // These should just be the property with the field:SerializeFieldAttribute set
    [Tooltip("Score multiplier if the quota is met.")]
    public float QuotaBonusMultiplier => quotaBonusMultiplier;
    public float PenaltyScoreMultiplier => penaltyScoreMultiplier;

    // This should use some special attribute so it's not editable in the inspector
    [SerializeField]
    float totalTime = 0f;

    [HideInInspector]
    public CollectorScheduler actingScheduler;

    [HideInInspector]
    public bool RandomiseRequiredTag => randomiseRequiredTag;
    public ScheduleQuota[] CollectionSchedule => collectionSchedule;
    public float TotalTime => totalTime;

    private void OnValidate()
    {
        float t = 0f;
        foreach (var r in collectionSchedule) t += r.timeLimit;
        totalTime = t;
    }

    /// <summary>
    /// Randomises each <see cref="ScheduleQuota.requiredTag"/> in <see cref="CollectionSchedule"/>. This is dependent on how many objects are
    /// in that array, and what <see cref="CrateTag"/>s are in that array. This function should only be called once - it will edit this asset's 
    /// data directly which may desynchronise other scripts which may depend on this data.
    /// </summary>
    public void RandomiseTags()
    {
        // Get a list of all the current tags in the schedule and randomise them
        List<CrateTag> tags = new();
        tags.AddRange(collectionSchedule.Select(quota => quota.requiredTag));
        ShuffleList(tags);

        // Apply shuffled tags
        for (int i = 0; i < collectionSchedule.Length; i++)
        {
            collectionSchedule[i].requiredTag = tags[i];
        }
    }
}