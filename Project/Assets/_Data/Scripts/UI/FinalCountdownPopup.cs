using System.Collections;
using UnityEngine;
using TMPro;

public class FinalCountdownPopup : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Reference in the scene to the Time Manager")]
	[SerializeField] private TimeManager timeManager;
	
	[Header("Cache")]
	[SerializeField] private Animator anim;
	[SerializeField] private TMP_Text timerText;
	
	private void Start()
	{
		// Subscribe to events
		timeManager.onTimerNearlyRanOut.AddListener(StartCountdown);
	}
	
	private void StartCountdown()
	{
		StartCoroutine(StartCountdownCoroutine());
	}
	
	// 10 ... 9 ... 8 etc
	private IEnumerator StartCountdownCoroutine()
	{
		anim.SetTrigger("Show");
		
		int finalCountdown = 30;
		
		while (finalCountdown > 0)
		{
			if (finalCountdown == 10)
			{
				anim.SetTrigger("Animate");
			}
			
			timerText.text = finalCountdown.ToString();
			
			finalCountdown--;
			
			yield return new WaitForSeconds(1.0f);
		}
		
		// Hide
		gameObject.SetActive(false);
	}
}