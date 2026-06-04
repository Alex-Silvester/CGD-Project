using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/AchievementCrateDestroyed")]
public class AchievementCrateDestroyed : Achievement
{
	void OnEnable()
    {
        // Subscribe to events
        CrateObject.onAnyDestroyed.AddListener(AddProgress);
    }
	
	// Needed to match event function signature
    void AddProgress()
    {
		IncreaseProgress();
    }
	
    void OnDisable()
    {
        // Unsubscribe from events
        CrateObject.onAnyDestroyed.RemoveListener(AddProgress);
    }
}