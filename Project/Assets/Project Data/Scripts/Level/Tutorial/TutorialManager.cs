using UnityEngine;
using UnityEngine.Events;
using TMPro;
using static CrateExtensions;

public class TutorialManager : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private TutorialStage[] tutorialStages;
	
	[Header("Cache")]
	[SerializeField] private TMP_Text instructionsMessageText;
	[SerializeField] private CollectionScheduleObject schedule;
	[SerializeField] private GameManager gameManager;
	
	private int currentTutorialStage = -1;
	private bool tutorialComplete;
	
	public static UnityEvent onTutorialComplete = new UnityEvent();
	
	private void Start()
	{
		// Make sure events trigger
		ProgressTutorial();
	}
	
	#region Setters
	
	public void ProgressTutorial()
	{
		if (tutorialComplete)
			return;
		
		if (currentTutorialStage >= 0)
			tutorialStages[currentTutorialStage].onCompleted?.Invoke();
		
		currentTutorialStage++;
		
		// Has the tutorial been completed?
		if (currentTutorialStage >= tutorialStages.Length)
		{
			tutorialComplete = true;
			
			onTutorialComplete?.Invoke();
			
			gameManager.SetVictoryState();
			
			return;
		}
		
		tutorialStages[currentTutorialStage].onBegun?.Invoke();
		
		// Setup new stage
		instructionsMessageText.text = tutorialStages[currentTutorialStage].instructionsMessage;
	}
	
	public void CheckTutorialCompleted(float currentScore)
	{		
		// Only one quota to complete tutorial
		if (currentScore >= schedule.CollectionSchedule[0].requiredScore)
		{
			ProgressTutorial();
		}
	}
	
	#endregion Setters
}

[System.Serializable]
public struct TutorialStage
{
	public string instructionsMessage;
	public UnityEvent onBegun;
	public UnityEvent onCompleted;
}