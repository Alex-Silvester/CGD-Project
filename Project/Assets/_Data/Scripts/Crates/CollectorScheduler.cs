using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CrateExtensions;

/// <summary>
/// Schedules the collection requirements for a given CrateCollector.
/// </summary>
[RequireComponent (typeof(Timer))]
public class CollectorScheduler : MonoBehaviour
{
    [SerializeField]
    Timer timer;

    [SerializeField]
    CollectionScheduleObject scheduleObject;

    [Tooltip("Events fired while the scheduler is running")]
    public UnityEvent SchedulerStarted, SchedulerEnded, SchedulerUpdated;

    /// <summary>
    /// Fired on update when this collector is processing through a schedule (i.e. Running = true).
    /// </summary>
    public Action<float> OnRunningUpdate;
    // This is an action because it will be fired each frame

    bool isRunning;

    public Queue<ScheduleQuota> Schedule { get; private set; }
    public ScheduleQuota CurrentRequirement { get; private set; }
    public float BonusQuotaMultipler { get; private set; }
    public bool Running => isRunning;
    public float RemainingQuotaTime => Mathf.Max(CurrentRequirement.timeLimit - timer.ElapsedTime, 0f);

    private void OnValidate()
    {
        if (!TryGetComponent(out timer))
        {
            Debug.LogWarning("CollectorScheduler cannot find its timer.");
        }
        else
        {
            timer.autoStart = false;
            timer.repeat = false;
        }

        if (scheduleObject == null)
        {
            Debug.LogWarning("CollectorScheduler does not have a schedule set in the inspector.");
        }
    }

    private void Awake()
    {
        isRunning = false;
        timer.repeat = false;
        timer.autoStart = false;
        BonusQuotaMultipler = 1f;
    }

    private void OnEnable()
    {
        timer.timeout.AddListener(UpdateSchedule);
    }

    private void OnDisable()
    {
        timer.timeout.RemoveListener(UpdateSchedule);
    }

    private void Update()
    {
        if (Running) OnRunningUpdate?.Invoke(RemainingQuotaTime);
    }

    /// <summary>
    /// Begin running through the collection schedule.
    /// </summary>
    public void StartScheduler()
    {
        isRunning = true;
        SchedulerStarted.Invoke();
        SetSchedule();
        UpdateSchedule();
    }

    void SetSchedule()
    {
        if (scheduleObject == null) return;
        if (scheduleObject.RandomiseRequiredTag) scheduleObject.RandomiseTags();

        Schedule = new Queue<ScheduleQuota>();

        foreach (var req in scheduleObject.CollectionSchedule)
        {
            Schedule.Enqueue(req);
        }
        BonusQuotaMultipler = scheduleObject.QuotaBonusMultiplier;
        scheduleObject.actingScheduler = this;
    }

    // Proceed through the schedule and handle what happens if the schedule is empty
    void UpdateSchedule()
    {
        if (TryGetNextInSchedule(out ScheduleQuota req))
        {
            // Process next item in schedule
            CurrentRequirement = req;
            timer.Restart();
            timer.duration = CurrentRequirement.timeLimit;
            timer.paused = false;
            SchedulerUpdated.Invoke();
        }
        else
        {
            // Completed the schedule
            isRunning = false;
            SchedulerEnded.Invoke();
            scheduleObject.actingScheduler = null;
        }
    }

    // Attempts to get a new requirement from the stack
    bool TryGetNextInSchedule(out ScheduleQuota requirement)
    {
        requirement = new ScheduleQuota();

        // Stack is empty (No more items in the schedule)
        if (Schedule.Count == 0) return false;

        requirement = Schedule.Dequeue();
        return true;
    }

}
