using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelPanel : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Level ScriptableObject in Project Folder this level panel will represent")]
	[SerializeField] private Level level;
	
	[Header("Cache")]
	[SerializeField] private TMP_Text levelNameText;
	[SerializeField] private Image levelScreenshot;
	[Tooltip("0 = one star, 1 = 2 star, 2 = 3 star")]
	[SerializeField] private GameObject[] stars;
	[SerializeField] private TMP_Text bestScoreText;

	[SerializeField] LoadingVariables loadingVariables;
	
	private void Start()
	{
		SetupUI();
	}
	
	private void SetupUI()
	{
		if (level == null)
		{
			Debug.Log("Level not set in Level Panel");
			return;
		}
		
		levelNameText.text = level.GetDisplayName();
		
		if (levelScreenshot)
			levelScreenshot.sprite = level.GetScreenshot();
		
		// Get score for this level (otherwise use the default zero text in the scene)
		if (SaveManager.instance != null && SaveManager.instance.currentSaveData != null)
		{
			float levelScore = SaveManager.instance.currentSaveData.scores.warehouse;
			
			bestScoreText.text = $"Best Score: {levelScore}";
			
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
    public void LoadScene()
    {
		if (level == null)
		{
			Debug.LogWarning("Level not set in Level Panel");
			return;	
		}
		
		if (level.GetSceneName() == string.Empty)
		{
			Debug.LogWarning("Tried to load scene without a name in Level Panel");
			return;	
		}
		
		loadingVariables.sceneName = level.GetSceneName();
		loadingVariables.activateScene();
    }
}