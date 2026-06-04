using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementInfo : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private Achievement achievement;
	
	[Header("Cache")]
	[SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_Text descriptionText;
	[SerializeField] private GameObject achievedMarker;
	
	private void Start()
	{
		Setup();
	}
	
	public void Setup()
	{
		if (!achievement)
			return;
		
		
		titleText.text = achievement.GetDisplayName();
		descriptionText.text = achievement.GetDescription();
		
		if (achievement.IsUnlocked())
			achievedMarker.SetActive(true);
	}
}