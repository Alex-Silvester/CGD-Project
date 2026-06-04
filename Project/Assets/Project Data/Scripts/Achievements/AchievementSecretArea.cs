using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/AchievementSecretArea")]
public class AchievementSecretArea : Achievement
{
	void OnEnable()
    {
        // Subscribe to events
        SecretAreaTrigger.onTriggered.AddListener(AddProgress);
    }
	
	// Needed to match event function signature
    void AddProgress()
    {
		IncreaseProgress();
    }
	
    void OnDisable()
    {
        // Unsubscribe from events
        SecretAreaTrigger.onTriggered.RemoveListener(AddProgress);
    }
}