using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private SceneMusic[] sceneMusic;
	
	[Header("Cache")]
	[SerializeField] private AudioSource audioSrc;
	
	private void Start()
	{
		// Prevent being destroyed between scenes
		DontDestroyOnLoad(gameObject); 
		
		// Subscribe to events
		SceneManager.activeSceneChanged += ChangedActiveScene;
	}
	
	// https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SceneManagement.SceneManager-activeSceneChanged.html
	private void ChangedActiveScene(Scene current, Scene next)
	{		
		// Is it a scene we need to change music for?
		foreach (var data in sceneMusic)
		{
			// Is it this one?
			if (next.name == data.sceneName)
			{
				// Make sure the same music isn't currently playing
				// E.g. we don't want to go into the options menu, then back to the main menu and restart the music
				if (data.musicClip != audioSrc.clip)
					ChangeMusic(data.musicClip);
			}
		}
	}
	
	private void ChangeMusic(AudioClip newMusic)
	{
		audioSrc.Stop();
		audioSrc.clip = newMusic;
		audioSrc.Play();
	}
	
	// Data container for the music that should play when this scene is loaded
	[System.Serializable]
	public struct SceneMusic
	{
		public string sceneName;
		public AudioClip musicClip;
	}
}