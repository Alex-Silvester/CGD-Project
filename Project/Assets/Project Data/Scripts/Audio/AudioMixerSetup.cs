using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Setups up the Audio Mixer values after the Save Manager has finished loading
/// </summary>
public class AudioMixerSetup : MonoBehaviour
{
	[SerializeField] private AudioMixer audioMixer;
	
    private void Start()
    {
        // Subscribe to events
		SaveManager.onLoaded.AddListener(Init);
    }
	
	private void Init()
	{
		// The slider is linear and the AudioMixer is logarithmic - see JLF comment - https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
		audioMixer.SetFloat("SFX", Mathf.Log(SaveManager.instance.currentSaveData.audio.sfxVolume) * 20);
		audioMixer.SetFloat("Music", Mathf.Log(SaveManager.instance.currentSaveData.audio.musicVolume) * 20);
		
	}
}