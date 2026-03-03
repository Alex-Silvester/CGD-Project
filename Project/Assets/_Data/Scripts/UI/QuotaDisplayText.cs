using TMPro;
using UnityEngine;


public class QuotaDisplayText : MonoBehaviour 
{
    [SerializeField]
    TextMeshProUGUI quotaReqs, quotaTimer;

    [SerializeField]
    CollectorScheduler scheduler;

    // The above serialisation feels wrong; maybe this should be set from an event?

    CrateExtensions.ScheduleQuota requirement;
    float trackedScore;

    string ReqsText => $"Crate: {requirement.requiredTag}\r\nQuota: {trackedScore}/{requirement.requiredScore}";
    string GetQuotaTimeString(float time) => $"Time: {time:F1}s";

    private void Awake()
    {
        trackedScore = 0f;
        HideText();
    }

    private void OnEnable()
    {
        if (scheduler == null) return;
        scheduler.OnRunningUpdate += OnRunningSchedulerUpdate;
    }

    private void OnDisable()
    {
        if (scheduler == null) return;
        scheduler.OnRunningUpdate -= OnRunningSchedulerUpdate;
    }

    // Assigned to event -> Update displated requirement and reset tracked score
    public void OnRequirementUpdated(CrateExtensions.ScheduleQuota requirement)
    {
        this.requirement = requirement;
        trackedScore = 0f;
        quotaReqs.SetText(ReqsText);
    }

    // Assigned to event -> Display the score when current collection score updates
    public void OnCollectionScoreUpdated(float score)
    {
        trackedScore = score;
        quotaReqs.SetText(ReqsText);
    }

    public void OnRunningSchedulerUpdate(float time)
    {
        quotaTimer.SetText(GetQuotaTimeString(time));
    }

    // Shows the text box
    public void ShowText() => gameObject.SetActive(true);

    // Hide the text box
    public void HideText() => gameObject.SetActive(false);
    
}
