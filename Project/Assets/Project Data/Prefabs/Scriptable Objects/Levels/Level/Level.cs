using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 1)]
/// <summary>
/// Scriptable Object representing level data
/// </summary>
public class Level : ScriptableObject
{
	[Tooltip("Name displayed to the player in the level select screen popup. E.g. Warehouse 1-1")]
	[SerializeField] private string displayName;
	[Tooltip("Name of the scene this level data is for. Must be in scene build list. E.g. BristolWarehouse")]
	[SerializeField] private string sceneName;
	[Tooltip("Reference in the Project Folder to the sprite that will be shown on the level select screen popup")]
	[SerializeField] private Sprite screenshot;
	[Tooltip("Scores required to achieve a star rating for this level.")]
	[SerializeField] private ScoreRequirements scoreRequirements;
	
	#region Getters
	
	public string GetDisplayName()
	{
		return displayName;
	}
	
	public string GetSceneName()
	{
		return sceneName;
	}
	
	public Sprite GetScreenshot()
	{
		return screenshot;
	}
	
	/// <summary>
	/// Get a level's score requirement for specified star level.
	/// Must be 1, 2, or 3.
	/// Return -1 is error value.
	/// </summary>
	public int GetScoreRequirement(int star)
	{
		switch (star)
		{
			case 1:
				return scoreRequirements.oneStar;
			case 2:
				return scoreRequirements.twoStar;
			case 3:
				return scoreRequirements.threeStar;
			default:
				Debug.Log("Tried to get invalid star for score requirement");
				return 0;
		}
	}
	
	public int GetStarRating(float score)
	{
		if (score >= scoreRequirements.threeStar)
			return 3;
		else if (score >= scoreRequirements.twoStar)
			return 2;
		else if (score >= scoreRequirements.oneStar)
			return 1;
		else
			return 0;
	}
	
	#endregion
}

/// <summary>
/// Data container representing scores required for each star rating on a level
/// </summary>
[System.Serializable]
public struct ScoreRequirements
{
	public int oneStar;
	public int twoStar;
	public int threeStar;
}