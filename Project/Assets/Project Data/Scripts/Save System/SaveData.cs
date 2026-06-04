using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make it appear in inspector
[System.Serializable]
/// <summary>
/// Template for save data file
/// Save data is broken down into sections to make finding variables easier
/// SaveManager will try and load an existing save data from disk when the game starts. If it does these variables will be filled out.
/// If there is no existing save then the default values will be used throughout the game
/// </summary>
public class SaveData
{
	// Containers for variables
	// Grouped to make it more manageable as we add more
	public AudioSaveData audio = new AudioSaveData();
	public AchievementSaveData achievements = new AchievementSaveData();
	public ScoreData scores = new ScoreData();
}

#region Options menu

[System.Serializable]
// Options audio menu variables
public class AudioSaveData
{
	public float sfxVolume = 1.0f;
	public float musicVolume = 0.3f;
}

#endregion Options menu

#region Achievements

[System.Serializable]
// Achievement variables
public class AchievementSaveData
{
	// Unity can't serialize Dictionaries
	public List<AchievementData> achievementData = new List<AchievementData>();
}

[System.Serializable]
public class AchievementData
{
	public string name; // Filename (not displayName), data
	public bool unlocked; // Tracked so we don't show "Achievement Unlocked" more than once
	public int currentProgress;
}

#endregion Achievements

#region Scores

[System.Serializable]
public class ScoreData
{
	// TODO use array instead of hardcoding
	// We only have one level so this isn't a problem (yet)
	public float warehouse = 0.0f;
}

#endregion Scores