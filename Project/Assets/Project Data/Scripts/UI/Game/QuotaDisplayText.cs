using TMPro;
using UnityEngine;


public class QuotaDisplayText : MonoBehaviour 
{
    [SerializeField]
    TextMeshProUGUI quotaReqs, quotaTimer;

	[SerializeField]
	Animator anim;

    [SerializeField]
    CollectorScheduler scheduler;

    // The above serialisation feels wrong; maybe this should be set from an event?

    CrateExtensions.ScheduleQuota requirement;
    float trackedScore;

    string ReqsText => $"COLLECT\n<color={requirement.requiredTag.ToString()}>{requirement.requiredTag} CRATES</color><line-height=50>\n</line-height>NEEDED\r\n{trackedScore}/{requirement.requiredScore}";
    string GetQuotaTimeString(float time) => $"{time:F0}s REMAINING";
	
	// Red text
	bool timeNearlyUpTriggered = false;
	static readonly float timeNearlyUpThreshold = 10.0f;

    private void Awake()
    {
        trackedScore = 0f;
        //HideText();
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
	
	private Color RequiredTagToColour(string tag)
	{
        switch (tag)
        {
        case "RED":
            return Color.red;
        case "GREEN":
            return Color.green;
        case "BLUE":
			return Color.blue;
        default:
			Debug.LogWarning(tag + " is not associated with a colour");
            return Color.white;
        }
	}

    // Assigned to event -> Update displated requirement and reset tracked score
    public void OnRequirementUpdated(CrateExtensions.ScheduleQuota requirement)
    {
        this.requirement = requirement;
        trackedScore = 0f;
        quotaReqs.SetText(ReqsText);
		
		// Reset red text
		timeNearlyUpTriggered = false;
		quotaTimer.color = Color.white;
		anim.SetBool("Animate", false);
    }

    // Assigned to event -> Display the score when current collection score updates
    public void OnCollectionScoreUpdated(float score)
    {
        trackedScore = score;
        quotaReqs.SetText(ReqsText);
    }

    public void OnRunningSchedulerUpdate(float time)
    {
		// Red text check
		if (!timeNearlyUpTriggered && time <= timeNearlyUpThreshold)
		{
			// Prevent triggering every frame
			timeNearlyUpTriggered = true;
			
			quotaTimer.color = Color.red;
			anim.SetBool("Animate", true);
		}
		
        quotaTimer.SetText(GetQuotaTimeString(time));
    }

    // Shows the text box
    public void ShowText() => gameObject.SetActive(true);

    // Hide the text box
    public void HideText() => gameObject.SetActive(false);
    
}
