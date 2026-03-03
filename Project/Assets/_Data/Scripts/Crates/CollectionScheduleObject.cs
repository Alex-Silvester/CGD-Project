using UnityEngine;
using static CrateExtensions;

/// <summary>
/// Contains the schedule for how a collector should collect crates
/// </summary>
[CreateAssetMenu(fileName = "CollectionScheduleObject", menuName = "Scriptable Objects/CollectionScheduleObject")]
public class CollectionScheduleObject : ScriptableObject
{
    [SerializeField]
    ScheduleQuota[] collectionSchedule;

    [SerializeField, Min(1f)]
    float quotaBonusMultiplier = 1f;

    public ScheduleQuota[] CollectionSchedule => collectionSchedule;

    [Tooltip("Score multiplier if the quota is met.")]
    public float QuotaBonusMultiplier => quotaBonusMultiplier;

    [HideInInspector]
    public CollectorScheduler actingScheduler;

    // This should use some special attribute so it's not editable in the inspector
    [SerializeField]
    float totalTime = 0f;

    public float TotalTime => totalTime;

    private void OnValidate()
    {
        float t = 0f;
        foreach (var r in collectionSchedule) t += r.timeLimit;
        totalTime = t;
    }
}