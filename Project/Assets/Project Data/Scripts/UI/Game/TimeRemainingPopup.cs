using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class TimeRemainingPopup : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Reference in the scene to the Time Manager")]
	[SerializeField] private TimeManager timeManager;
	[Tooltip("Time between showing the popup")]
	[SerializeField] private float countdownInterval = 60.0f;
	
	[Header("Cache")]
	[SerializeField] private Animator anim;
	[SerializeField] private TMP_Text timerText;
	
	private int minutesRemaining;
	
	private void Start()
	{
		// Wait until after CurrentTimeRemaining is set (there is a delay for Scheduler)
		StartCoroutine(SetMinutesRemainingCoroutine());
	}
	
	private IEnumerator SetMinutesRemainingCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
		
		// Convert float (seconds) to minutes
		// Source - https://discussions.unity.com/t/convert-float-to-time-minutes-and-seconds/742908/10
		var ts = TimeSpan.FromSeconds(timeManager.CurrentTimeRemaining);
		minutesRemaining = ts.Minutes;
		
		StartCoroutine(CountdownCoroutine());
	}
	
	private IEnumerator CountdownCoroutine()
	{
		// Loop until level ends
		while (minutesRemaining > 0)
		{
			yield return new WaitForSeconds(countdownInterval);
			
			Show();
		}
	}
	
	private void Show()
	{
		anim.SetTrigger("Show");
		
		// Plural
		if (minutesRemaining > 1)
			timerText.text = $"{minutesRemaining} MINUTES UNTIL SHIFT END";
		// Singular
		else
			timerText.text = $"{minutesRemaining} MINUTE UNTIL SHIFT END";
		
		minutesRemaining--;
		
		// This is always 1 minute out
		
		// Format with minutes instead of float
		//var ts = TimeSpan.FromSeconds(timeManager.CurrentTimeRemaining);
		//timerText.text = string.Format("{0:0} minutes until shift end", ts.Minutes);
	}
	
	private void Hide()
	{
		anim.SetTrigger("Hide");
	}
}