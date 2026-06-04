using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/AchievementTest")]
public class AchievementTest : Achievement
{
	void OnEnable()
    {
        // Subscribe to events
        CheatCode.onCheatActivated.AddListener(AddProgress);
    }
	
	// Needed to match event function signature
    void AddProgress()
    {
		IncreaseProgress();
    }
	
    void OnDisable()
    {
        // Unsubscribe from events
        CheatCode.onCheatActivated.RemoveListener(AddProgress);
    }
}