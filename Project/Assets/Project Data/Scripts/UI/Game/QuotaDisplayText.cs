using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class QuotaDisplayText : MonoBehaviour 
{
    [SerializeField] TextMeshProUGUI quotaReqs, quotaProgressText, quotaTimeText;
    [SerializeField] Slider quotaTimer, quotaProgressBar;
    [SerializeField] Image progressImage;

    [SerializeField] Image background;
    [SerializeField] Sprite redCentre, greenCentre, blueCentre;

	[SerializeField]
	Animator anim;

    [SerializeField]
    CollectorScheduler scheduler;

    // The above serialisation feels wrong; maybe this should be set from an event?

    CrateExtensions.ScheduleQuota requirement;
    float trackedScore;

    string ReqsText => $"<color={requirement.requiredTag.ToString()}>>{requirement.requiredTag} Crates<";
    string progressText => $"{trackedScore}/{requirement.requiredScore}";
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

        updateProgress();
        
		// Reset red text
		timeNearlyUpTriggered = false;
		anim.SetBool("Animate", false);
    }

    // Assigned to event -> Display the score when current collection score updates
    public void OnCollectionScoreUpdated(float score)
    {
        trackedScore = score;
        quotaProgressBar.value = score / requirement.requiredScore;
        quotaProgressText.SetText(progressText);
    }

    public void OnRunningSchedulerUpdate(float time)
    {
		// Red text check
		if (!timeNearlyUpTriggered && time <= timeNearlyUpThreshold)
		{
			// Prevent triggering every frame
			timeNearlyUpTriggered = true;
			
			anim.SetBool("Animate", true);
		}
		
        quotaTimer.value = time / requirement.timeLimit;
        quotaTimeText.SetText(GetQuotaTimeString(time));
    }

    public void updateProgress()
    {
        trackedScore = 0f;
        quotaReqs.SetText(ReqsText);
        quotaProgressText.SetText(progressText);

        quotaProgressBar.value = 0f;

        if (requirement.requiredTag == CrateExtensions.CrateTag.Red)
        {
            progressImage.color = Color.red;
            background.sprite = redCentre;
        }
        else if (requirement.requiredTag == CrateExtensions.CrateTag.Green)
        {
            progressImage.color = Color.green;
            background.sprite = greenCentre;
        }
        else
        {
            progressImage.color = Color.blue;
            background.sprite = blueCentre;
        }
    }

    // Shows the text box
    public void ShowText() => gameObject.SetActive(true);

    // Hide the text box
    public void HideText() => gameObject.SetActive(false);
    
}
