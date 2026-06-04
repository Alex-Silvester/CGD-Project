using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/AchievementCrateReachedPeak")]
public class AchievementCrateReachedPeak : Achievement
{
	void OnEnable()
    {
        // Subscribe to events
        PeakDetection.onCrateReached.AddListener(AddProgress);
    }
	
	// Needed to match event function signature
    void AddProgress()
    {
		IncreaseProgress();
    }
	
    void OnDisable()
    {
        // Unsubscribe from events
        PeakDetection.onCrateReached.RemoveListener(AddProgress);
    }
}