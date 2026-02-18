using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
	// Events
	public UnityEvent onTimerStarted = new UnityEvent();
	// Timer has been paused
	public UnityEvent onTimerStopped = new UnityEvent();
	// Time has nearly run out. Encourage the player to hurry up.
	public UnityEvent onTimerNearlyRanOut = new UnityEvent();
	// The level time has ran out and the level with end
	public UnityEvent onTimerRanOut = new UnityEvent();

	// Current schedule - used to get timer duration
	[SerializeField]
	private CollectionScheduleObject currentSchedule;
	
	public bool TimerActive { get; private set; } = true;
	// Time until the level ends
    public float CurrentTimeRemaining { get; private set; }
	// Encourage players to hurry up with red text
	public readonly static float timerNearlyRanOutThreshhold = 15.0f;
	
	// Make sure event is only called once
	private bool timerNearlyRunOutTriggered = false;
	
    // Singleton
    public static TimeManager instance { get; private set; }
	public CollectionScheduleObject CurrentScheduleObject => currentSchedule;
	
	/// <summary>
	/// Setup a Singeton so there is only one TimeManager that can be acccessed in any other script
	/// </summary>
	private void Awake()
	{
		// Assign Singleton
		instance = this;
	}
	
	private void Start()
	{
        // Set time to total schedule time
        // (adding +0.01 so the timer events here are PROBABLY gonna fire after the collector's timer events fire; this definitely doesn't work if the fps is low but oh well Lole)
        CurrentTimeRemaining = currentSchedule.TotalTime + 0.01f;
		
		// Subscribe to events
		GameOverState.onEntered += StopTimer;
		VictoryState.onEntered += StopTimer;
	}
	
	/// <summary>
	/// While timer is active continually countdown time until reaching zero
	/// </summary>
	private void Update()
	{
		// Timer countdown
		if (TimerActive)
		{
			if (CurrentTimeRemaining > 0)
			{
				CurrentTimeRemaining -= Time.deltaTime;
				
				// Should hurry up warning be triggered
				if (!timerNearlyRunOutTriggered)
				{
					if (CurrentTimeRemaining <= timerNearlyRanOutThreshhold)
					{
						// Prevent triggering event twice
						timerNearlyRunOutTriggered = true;
						
						// Let other scripts know
						onTimerNearlyRanOut?.Invoke();
					}
				}
				
				// Check if time is up
				if (CurrentTimeRemaining <= 0)
				{
					// Let other scripts know
					onTimerRanOut?.Invoke();
				}
			}
		}
	}
	
	#region Setters
	
	public void StartTimer()
	{
		// Make countdown continue
		TimerActive = true;
		
		// Let other scripts know
		onTimerStarted?.Invoke();
		
		// Reset hurry up trigger bool
		timerNearlyRunOutTriggered = false;
		
		Debug.Log("Start timer");
	}
	
	// Pause timer
	public void StopTimer()
	{
		// Pause countdown
		TimerActive = false;
		
		// Let other scripts know
		onTimerStopped?.Invoke();
	}
	
	public void ToggleTimer()
	{
		if (TimerActive)
			StopTimer();
		else
			StartTimer();
	}
	
	#endregion Setters
	
	private void OnDestroy()
	{
		// Unsubscribe from events
		GameOverState.onEntered -= StopTimer;
		VictoryState.onEntered -= StopTimer;
	}
}