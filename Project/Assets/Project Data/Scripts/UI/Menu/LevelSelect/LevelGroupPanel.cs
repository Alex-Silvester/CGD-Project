using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelGroupPanel : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("For getting star rating requirements")]
	[SerializeField] private Level level; // Temporary quick fix as we only have one level anyway
	[Tooltip("Level Group ScriptableObject in Project Folder this level group panel will represent")]
	[SerializeField] private LevelGroup levelGroup;
	[Tooltip("Reference in the scene to the individual level container for this level group")]
	[SerializeField] private GameObject levelContainer;
	[Tooltip("Reference in the scene to the group container for this level group")]
	[SerializeField] private GameObject levelGroupContainer;
	
	[Header("Cache")]
	[SerializeField] private TMP_Text levelGroupNameText;
	[SerializeField] private Image levelGroupScreenshot;
	[SerializeField] private TMP_Text totalScoreText;
	[SerializeField] private GameObject[] stars;
	
	private void Start()
	{
		SetupUI();
	}
	
	private void SetupUI()
	{
		if (levelGroup == null)
		{
			Debug.Log("Level Group not set in Level Group Panel");
			return;
		}
		
		levelGroupNameText.text = levelGroup.GetDisplayName();
		
		if (levelGroupScreenshot)
			levelGroupScreenshot.sprite = levelGroup.GetScreenshot();
		
		// Get score for this level (otherwise use the default zero text in the scene)
		if (SaveManager.instance != null && SaveManager.instance.currentSaveData != null)
		{
			float levelScore = SaveManager.instance.currentSaveData.scores.warehouse;
			
			totalScoreText.text = $"Total Score: {levelScore}";
			
			SetupStars(levelScore);
		}
	}
	
	private void SetupStars(float levelScore)
	{
		if (!level)
			return;
		
		
		// Calculate star rating
		float starRating = level.GetStarRating(levelScore);
		
		for (int i = 0; i < starRating; i++)
		{
			stars[i].SetActive(true);
		}
	}
	
	// Use for button onclicked event
	public void Disable()
	{
		if (levelGroup == null)
		{
			Debug.LogWarning("Level Group not set in Level Group Panel");
			return;	
		}
		
		if (levelContainer == null)
		{
			Debug.LogWarning("Tried to load scene without a name in Level Group Panel");
			return;	
		}
		
        levelContainer.SetActive(true);
		levelGroupContainer.SetActive(false);
	}
	
	public void Enable()
	{
        levelContainer.SetActive(false);
		levelGroupContainer.SetActive(true);
	}
}