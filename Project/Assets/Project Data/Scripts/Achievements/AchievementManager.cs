using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AchievementManager : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Achievements in the game")]
	[SerializeField] private Achievement[] achievements;
	
	// Singleton
	private static AchievementManager instance;
	
	/// <summary>
	/// Setup a Singeton so there is only one SaveManager that can be acccessed in any other script
	/// </summary>
	private void Awake()
	{
		// Check for existing Singleton
		if (instance == null)
		{
			// Assign Singleton
			instance = this;
			
			// Survive scene change
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			// There can only be one save manager
			Destroy(this);
			
			Debug.LogWarning("Tried to create a second Achievement Manager. Only one is allowed.");
		}
	}
}