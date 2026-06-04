using UnityEngine;
using UnityEngine.Events;

public class AchievementProgressor : MonoBehaviour
{
	[SerializeField] private Achievement achievement;
	[SerializeField] [Min(1)] private int progressAmount = 1;
	
	public void IncreaseProgress()
	{
		if (!achievement)
			return;
		
		// Will handle IsUnlocked check itself
		achievement.IncreaseProgress(progressAmount);
	}
}