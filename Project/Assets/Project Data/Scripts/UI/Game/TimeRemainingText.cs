using System;
using UnityEngine;
using TMPro;

// NOTE: The TimeRemainingPanel this is a part of should be its own prefab.
//       Possible solution -> make a prefab for 'generic panel' and then make a prefab variant

public class TimeRemainingText : MonoBehaviour
{
	[Header("Cache")]
	[Tooltip("Reference to the time remaining text in the scene")]
	[SerializeField] private TMP_Text currentTimeRemainingText;
	[Tooltip("Reference to the Animator component for the text")]
	[SerializeField] private Animator anim;
	
	private void Start()
	{
		// Subscribe to events
		TimeManager.instance.onTimerNearlyRanOut.AddListener(OnHurryUp);
		TimeManager.instance.onTimerRanOut.AddListener(OnTimeRanOut);
	}
	
	private void Update()
	{
		// TODO: Move this to OnValidate (if this happens while running not much you can do (plus I think Debug functions use a lot of memory))
		if (!currentTimeRemainingText)
		{
			Debug.LogWarning("Current time remaining text not set in the inspector.");
			return;
		}
		
		if (!TimeManager.instance)
		{
			Debug.LogWarning("Time Manager instance not found.");
			return;
		}
		
		// Update text to one decimal place (the decimal place makes the number appear to be decreasing faster than it really is)
		if (TimeManager.instance.CurrentTimeRemaining > 0)
		{
			// Convert float (seconds) to minutes and seconds remaining
			// Source - https://discussions.unity.com/t/convert-float-to-time-minutes-and-seconds/742908/10
			var ts = TimeSpan.FromSeconds(TimeManager.instance.CurrentTimeRemaining);
			
			// E.g. 6:07
			currentTimeRemainingText.text = "TIME " + string.Format("{0}:{1:00}", ts.Minutes, ts.Seconds);
		}
		else
		{
			currentTimeRemainingText.text = "TIME'S UP!";
		}
	}
	
	private void OnHurryUp()
	{
		// Make text stand out and grab player attention
		currentTimeRemainingText.color = Color.red;
		
		// Play pulsate text animation
		if (anim)
			anim.SetBool("IsAnimating", true);
	}
	
	private void OnTimeRanOut()
	{
		// Prevent time text being distracting after level ended
		currentTimeRemainingText.color = Color.white;
		
		// Stop pulsate text animation
		if (anim)
			anim.SetBool("IsAnimating", false);
	}
	
	private void OnDestroy()
	{
		// Unsubscribe from events
		TimeManager.instance.onTimerNearlyRanOut.RemoveListener(OnHurryUp);
		TimeManager.instance.onTimerRanOut.RemoveListener(OnTimeRanOut);
	}
}