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
	[SerializeField] private Image[] starRatingImages;
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