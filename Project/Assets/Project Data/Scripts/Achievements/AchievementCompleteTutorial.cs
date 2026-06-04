using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/AchievementCompleteTutorial")]
public class AchievementCompleteTutorial : Achievement
{
	void OnEnable()
    {
        // Subscribe to events
        TutorialManager.onTutorialComplete.AddListener(AddProgress);
    }
	
	// Needed to match event function signature
    void AddProgress()
    {
		IncreaseProgress();
    }
	
    void OnDisable()
    {
        // Unsubscribe from events
        TutorialManager.onTutorialComplete.RemoveListener(AddProgress);
    }
}