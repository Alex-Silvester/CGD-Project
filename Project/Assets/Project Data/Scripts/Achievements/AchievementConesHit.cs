using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/AchievementConesHit")]
public class AchievementConesHit : Achievement
{
	void OnEnable()
    {
        // Subscribe to events
        TrafficCone.onAllHit.AddListener(AddProgress);
    }
	
	// Needed to match event function signature
    void AddProgress()
    {
		IncreaseProgress();
    }
	
    void OnDisable()
    {
        // Unsubscribe from events
        TrafficCone.onAllHit.RemoveListener(AddProgress);
    }
}