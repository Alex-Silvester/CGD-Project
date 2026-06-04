using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/Achievement")]
// Use the base class if you are going to have a reference to this and call IncreaseProgress in another script
// Otherwise, create a base class (see AchievementTest)
public class Achievement : ScriptableObject
{	
	[Header("Settings")]
	[SerializeField] private string displayName;
	[SerializeField] private string description;
	[Tooltip("When the achievement reaches this amount of progress, it will unlock")]
	[SerializeField] private int requiredProgress = 1;
	
	private void CheckRequirementsMet()
	{
		if (!SaveManager.instance)
		{
			Debug.LogWarning("Achievement couldn't find Save Manager instance");
			return;
		}
		
		// Is there any Save Data for this achievement?
		if (GetAchievementData(name) == null)
			return;
		
		
		
		// Prevent unlocking more than once
		if (GetAchievementData(name).unlocked)
			return;
		
		
		if (GetAchievementData(name).currentProgress >= requiredProgress)
		{
			Unlock();
		}
	}
	
	private void Unlock()
	{
		if (!SaveManager.instance)
		{
			Debug.LogWarning("Achievement couldn't find Save Manager instance");
			return;
		}
		
		
		
		// Prevent unlocking more than once
		if (IsUnlocked())
			return;
		
		
		if (GetAchievementData(name) != null)
		{
			GetAchievementData(name).unlocked = true;
		}
		
		// Persist to disk
		if (SaveManager.instance)
			SaveManager.instance.Save();
		
		// Show achievement unlocked notification
		if (ToastNotificationManager.instance)
			ToastNotificationManager.instance.Add("Trophy Unlocked", displayName);
		else
			Debug.LogWarning("Achievement couldn't find Toast Notification Manager");
	}
	
	#region Getters
	
	public string GetDisplayName()
	{
		return displayName;
	}
	
	public string GetDescription()
	{
		return description;
	}
	
	public int GetRequiredProgress()
	{
		return requiredProgress;
	}
	
	public bool IsUnlocked()
	{
		if (!SaveManager.instance)
		{
			Debug.LogWarning("Achievement couldn't find Save Manager instance");
			return false;
		}
		
		
		
		// Is there any Save Data for this achievement?
		if (GetAchievementData(name) != null)
		{
			return GetAchievementData(name).unlocked;
		}
		
		// Since there is none it must be false
		return false;
	}
	
	public int GetCurrentProgress()
	{
		if (!SaveManager.instance)
		{
			Debug.LogWarning("Achievement couldn't find Save Manager instance");
			return 0;
		}
		
		
		// Is there any Save Data for this achievement?
		if (GetAchievementData(name) != null)
		{
			return GetAchievementData(name).currentProgress;
		}
		
		// Since there is none it must be zero
		return 0;
	}
	
	#endregion
	
	#region Setters
	
	public void IncreaseProgress(int amount = 1)
	{		
		if (!SaveManager.instance)
		{
			Debug.LogWarning("Achievement couldn't find Save Manager instance");
			return;
		}
				
		// Is there any Save Data for this achievement?
		if (GetAchievementData(name) != null)
		{			
			GetAchievementData(name).currentProgress += amount;
		}
		// Create some new Save Data
		else
		{			
			// Create
			AchievementData data = new AchievementData();
			data.name = name;
			data.currentProgress = amount;
			
			// Submit
			SaveManager.instance.currentSaveData.achievements.achievementData.Add(data);
		}
				
		// Have we progressed enough to unlock the achievement?
		CheckRequirementsMet();
		
		// Persist to disk
		if (SaveManager.instance)
			SaveManager.instance.Save();
	}
	
	#endregion
	
	#region Utility
	
	private AchievementData GetAchievementData(string achievementName)
	{
		if (!SaveManager.instance)
		{
			Debug.LogWarning("Achievement couldn't find Save Manager instance");
			return null;
		}
		
		
		
		// Search through list of achievements (Unity doesn't support serializing Dictionaries)
		foreach (var achievement in SaveManager.instance.currentSaveData.achievements.achievementData)
		{
			// Is it this one?
			if (achievement.name == achievementName)
			{
				return achievement;
			}
		}
		
		// Nothing found
		return null;
	}
	
	#endregion
}