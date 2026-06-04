using UnityEngine;

public class AchievementProgressorSlider : AchievementProgressor
{
	[SerializeField] private float requiredValue;
	
	public void Check(float value)
	{
		if (value == requiredValue)
			IncreaseProgress();
	}
}