using UnityEngine;

public class QuotaCompletePopup : MonoBehaviour
{
	[Header("Local Cache")]
	[SerializeField] private Animator panelAnimator;
	
	[Header("Object Cache")]
	[SerializeField] private CrateCollector crateCollector;
	
	private void Start()
	{
		// Subscribe to events
		crateCollector.onEvaluatedRequirement.AddListener(OnQuotaEnded);
	}
	
	private void OnQuotaEnded(bool isCompleted)
	{
		if (isCompleted)
		{
			panelAnimator.SetTrigger("Show");
		}
	}
}