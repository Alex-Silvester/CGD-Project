using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static CrateExtensions;

/// <summary>
/// MonoBehaviour that uses the GameObject's collider to detect and collect crates.
/// </summary>
[RequireComponent(typeof(CollectorScheduler))]
public class CrateCollector : MonoBehaviour
{
    [SerializeField]
    ScoreObject scoreObject;

    [SerializeField]
    CollectorScheduler scheduler;

    [SerializeField]
    AudioEnabler audioEnabler;

    [Space, Header("Settings")]

    [SerializeField, Range(0f, 100f)]
    float rejectionLaunchForce = 10f;

    [SerializeField, Range(0f, 100f)]
    float acceptLaunchForce = 5f;

    [Space, Header("Event Bindings")]

    [SerializeField]
    UnityEvent<ScheduleQuota> onRequirementUpdate;

    [SerializeField]
    UnityEvent<float> onItemsForCollectionChanged;

    [SerializeField]
    UnityEvent onCollectionPeriodStarted, onCollectionPeriodEnded;

    [SerializeField]
    public UnityEvent<bool> onEvaluatedRequirement;

    float currentCollectionScore = 0f;
    bool canCollect = false;
    bool wasStarted = false;
    List<ICollectable> forCollection;
    ScheduleQuota collectionRequirement;

    // Get vector for launching a crate
    Vector3 GetLaunchForce(float magnitude) => transform.forward * magnitude + new Vector3(0f, 5f, 0f);

    // Returns predicted score
    float GetScoreWaitingInCollection() => forCollection.Sum(item => item.Score);

    private void OnValidate()
    {
        // Should I even bother doing this?
        try
        {
            audioEnabler = GetComponentInChildren<AudioEnabler>();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    void Initialise()
    {
        if (!TryGetComponent(out Collider _))
        {
            Debug.LogWarning("Collector is missing a collider.");
        }
         
        forCollection = new List<ICollectable>();
    }


    private void Awake()
    {
        Initialise();
        scoreObject.SetScore(0f);
    }

    private void OnEnable()
    {
        onEvaluatedRequirement.AddListener(ProcessSuccessFailAudio);
        scheduler.SchedulerStarted.AddListener(DoCollect);
        scheduler.SchedulerUpdated.AddListener(UpdateFromSchedule);
        scheduler.SchedulerEnded.AddListener(NoCollect);
        scheduler.SchedulerEnded.AddListener(EvaluateRequirement);
    }
    private void OnDisable()
    {
        onEvaluatedRequirement.RemoveListener(ProcessSuccessFailAudio);
        scheduler.SchedulerStarted.RemoveListener(DoCollect);
        scheduler.SchedulerUpdated.RemoveListener(UpdateFromSchedule);
        scheduler.SchedulerEnded.RemoveListener(NoCollect);
        scheduler.SchedulerEnded.RemoveListener(EvaluateRequirement);
    }

    private void Start()
    {
        StartCollector();
    }


    // If other is a collectable add it to list
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICollectable collectable))
        {
            TryCollect(collectable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ICollectable collectable))
        {
            // RemoveCollectable(collectable);
        }
    }

    // Will attempt to collect the given collectable
    void TryCollect(ICollectable collectable)
    {
        // Nothing is collectable
        if (!canCollect || collectable.CanCollect == false) return;

        if (collectable.Tag == collectionRequirement.requiredTag && !forCollection.Contains(collectable))
        {
            forCollection.Add(collectable);
            onItemsForCollectionChanged.Invoke(GetScoreWaitingInCollection());
            collectable.CanDamage = false;
            collectable.GameObject.GetComponent<Rigidbody>().AddForce(-GetLaunchForce(acceptLaunchForce), ForceMode.Impulse);
        }
        else if (collectable.Tag != collectionRequirement.requiredTag)
        {
            collectable.GameObject.GetComponent<Rigidbody>().AddForce(GetLaunchForce(rejectionLaunchForce) + new Vector3(0f, 0f, 0f), ForceMode.Impulse);
        }

    }

    // Remove collectable from the list
    void RemoveCollectable(ICollectable collectable)
    {
        if (canCollect && forCollection.Contains(collectable))
        {
            forCollection.Remove(collectable);
            collectable.CanDamage = true;
            onItemsForCollectionChanged.Invoke(GetScoreWaitingInCollection());
        }
    }

    // Starts the collector for collecting
    public void StartCollector()
    {
        wasStarted = true;
        scheduler.StartScheduler();
    }

    // Set the current collection requirement to what is currently scheduled
    void UpdateRequirement()
    {
        if (!scheduler.Running) return;

        collectionRequirement = scheduler.CurrentRequirement;
        onRequirementUpdate.Invoke(collectionRequirement);
    }

    // Collects the crate (removes the object and adds some score)
    private void CollectCrate(ICollectable collectable)
    {
        currentCollectionScore += collectable.Score;
        Destroy(collectable.GameObject);
    }

    // Check if the current requirement was met
    void EvaluateRequirement()
    {
        // Collect everything that should be collected
        foreach(var c in forCollection) CollectCrate(c);
        bool isSuccess = (currentCollectionScore >= collectionRequirement.requiredScore);
        currentCollectionScore *= isSuccess ? scheduler.BonusQuotaMultipler : 1f;

        // Add score + invoke events
        scoreObject.AddScore(currentCollectionScore);
        onEvaluatedRequirement.Invoke(isSuccess);
        currentCollectionScore = 0f;

        forCollection.Clear();
    }

    // For invocation whenever the schedule changes the current collection requirement
    void UpdateFromSchedule()
    {
        // Call UpdateRequirement after evaluating the current requirement
        // If this was called when the collector first started, don't evaluate (nothing to check)
        if (!wasStarted)
        {
            EvaluateRequirement();
        }
        else
        {
            wasStarted = false;
        }
        UpdateRequirement();
    }

    // Make this collect or not
    internal void SetCollection(bool can)
    {
        canCollect = can;

        if (can)
        {
            onCollectionPeriodStarted.Invoke();
        }
        else
        {
            onCollectionPeriodEnded.Invoke();
        }
    }

    // Variants of above but always true/false
    void DoCollect() => SetCollection(true);
    void NoCollect() => SetCollection(false);

    // Play successs or fail audio only if the state is in playing
    void ProcessSuccessFailAudio(bool pass)
    {
        if (audioEnabler == null)
        {
            Debug.LogWarning("Crate collector does not have an AudioEnabler to play sounds from");
            return;
        }

        if (GameManager.instance.currentState == GameManager.instance.playingState)
        {
            audioEnabler.Enable(pass ? "Pass" : "Fail");
            return;
        }
        audioEnabler.Enable("Ended");
    }
}
