using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static CrateExtensions;

// Crate launching doesn't do anything now but I'm still keeping the code here in the event we want to revert the changes

/// <summary>
/// MonoBehaviour that uses the GameObject's collider to detect and collect crates.
/// </summary>
[RequireComponent(typeof(CollectorScheduler))]
public class CrateCollector : MonoBehaviour
{
    [SerializeField]
    ScoreObject scoreObject;

    [SerializeField]
    ParticleEffectLibrary effectLibrary;

    [SerializeField]
    CollectorScheduler scheduler;

    [SerializeField]
    AudioEnabler audioEnabler;

    [SerializeField]
    ArrayArrangement gridArrangement;

    [SerializeField]
    AudioClip confettiSound;

    [SerializeField]
    Vector2 pitchRandomise = new(-0.05f, 0.05f);

    [SerializeField, Tooltip("Confetti rotation is based on the forward vector.")]
    Transform[] confettiTransform;

    [Space, Header("Event Bindings")]
    public UnityEvent<ScheduleQuota> onRequirementUpdate;
    public UnityEvent<float> onItemsForCollectionChanged;
    public UnityEvent onCollectionPeriodStarted, onCollectionPeriodEnded;
    public UnityEvent<bool> onEvaluatedRequirement;

    float currentCollectionScore = 0f;
    bool canCollect = false;
    bool wasStarted = false;
    List<ICollectable> forCollection;
    ScheduleQuota collectionRequirement;

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
        if (confettiTransform.Length == 0)
        {
            confettiTransform = new Transform[1];
            confettiTransform[0] = transform;
        }
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

    // Will attempt to collect the given collectable
    void TryCollect(ICollectable collectable)
    {
        // Nothing is collectable or can be collected
        if (!canCollect || collectable.CanCollect == false || forCollection.Contains(collectable)) return;

        // Apply penalty multiplier if the collectable doesn't match the quota
        // If the multiplier is negative it may potentially destroy the crate so it should exit early
        if (collectable.Tag != collectionRequirement.requiredTag)
        {
            collectable.Score = (int)(collectable.Score * scheduler.PenaltyScoreMultiplier);
            if (collectable.Score <= 0) return;
        }

        collectable.CanDamage = collectable.CanCollect = false;
        forCollection.Add(collectable);
        gridArrangement.Add(collectable.GameObject.transform);
        DisplayConfettiParticles(collectable.Tag.GetColourFromTag());
        onItemsForCollectionChanged.Invoke(GetScoreWaitingInCollection());
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
        gridArrangement.Remove(collectable.GameObject.transform);
        Destroy(collectable.GameObject);
    }

    // Check if the current requirement was met
    void EvaluateRequirement()
    {
        // Collect everything that should be collected
        foreach(var c in forCollection) CollectCrate(c);
        bool isSuccess = (currentCollectionScore >= collectionRequirement.requiredScore);
        if (isSuccess)
        {
            currentCollectionScore *= scheduler.BonusQuotaMultipler;
            currentCollectionScore = Mathf.FloorToInt(currentCollectionScore);
            DisplayConfettiParticles(collectionRequirement.requiredTag.GetColourFromTag());
        }

        // Add score + invoke events
        scoreObject.AddScore(currentCollectionScore);
        onEvaluatedRequirement.Invoke(isSuccess);
        currentCollectionScore = 0f;

        forCollection.Clear();
    }

    void DisplayConfettiParticles(Color color)
    {
        bool playSound = true;
        foreach (Transform point in confettiTransform)
        {
            var confetti = effectLibrary.Get<ColoredParticleEffect>(ParticleEffectLibrary.Confetti);
            confetti.color = confetti.emissionColor = color;
            if (playSound)
            {
                confetti.WithSound(confettiSound, pitchRandomise: pitchRandomise, spatialBlend: 1f);
                playSound = false;
            }
            confetti.AtPosition(point.position)
                    .AtRotation(point.rotation)
                    .Play();
        }
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
