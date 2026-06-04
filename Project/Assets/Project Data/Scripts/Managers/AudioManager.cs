using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	[Header("Cache")]
	[Tooltip("Reference to AudioMixer in assets")]
	[SerializeField] private AudioMixer audioMixer;
	
	private void Start()
	{
		// Subscribe to events
		SaveManager.onLoaded.AddListener(OnLoaded);
		
		// Survive scene changes
		DontDestroyOnLoad(gameObject);
	}

	// Save file has finished loading, so it's safe to fill in values
	private void OnLoaded()
	{
		// The slider is linear and the AudioMixer is logarithmic - see JLF comment - https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
		audioMixer.SetFloat("SFX", Mathf.Log( SaveManager.instance.currentSaveData.audio.sfxVolume ) * 20);
		
		// Change for this playthrough
        audioMixer.SetFloat("Music", Mathf.Log( SaveManager.instance.currentSaveData.audio.musicVolume ) * 20);
	}
	
	private void OnDestroy()
	{
		// Unsubscribe from events
		SaveManager.onLoaded.RemoveListener(OnLoaded);
	}
}