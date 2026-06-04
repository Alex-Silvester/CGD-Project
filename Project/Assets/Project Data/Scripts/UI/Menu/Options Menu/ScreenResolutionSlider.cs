using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// Set the game's screen resolution to an index value in Screen.resolutions array
/// Float is used because slider OnValueChanged only accepts dynamic as a float. It will be converted to an int inside the SetResolutionLevel function.
/// Note: Unity automatically saves any change in resolution or fullscreen to PlayerPrefs - https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Screen.html
/// Source - https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Screen-resolutions.html
/// Source - https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Screen.SetResolution.html
/// </summary>
public class ScreenResolutionSlider : MonoBehaviour
{
	[Header("Cache")]
	[Tooltip("Reference to the UI slider in the scene.")]
    [SerializeField] Slider slider;
	[Tooltip("Reference to the text in the scene that will display the resolution setting (e.g. 1920x1080).")]
	[SerializeField] TMP_Text resolutionSettingText;
	
	private void Start()
	{
		// Set the slider maximum value to match screen resolution setting quantity
		// -1 to match array index
		slider.maxValue = Screen.resolutions.Length - 1;
		
        // Init setup
		// TODO load from save code
		// Find the game's current resolution, then set the screen resolution slider to the correct notch
        Resolution[] resolutions = Screen.resolutions;

        // Loop through all the available screen resolutions
		for(int i = 0; i < resolutions.Length; i++)
		{
			// Is this the game's current screen resolution?
			if (Screen.width == resolutions[i].width && Screen.height == resolutions[i].height)
            {
				// Update slider notch position
				slider.value = i;
				
				// Update slider label
				UpdateSliderText(i);
				
				break;
			}
		}
	}
	
	/// <summary>
	/// Change the game's display resolution to a Screen.resolutions array index
	/// Parameter must be float as slider OnValueChanged only accepts dynamic as a float. It will be converted to an int inside the function.
	/// </summary>
	public void SetResolutionLevel(float sliderValue)
	{
		// Convert the slider value the type QualitySetting requires
		int newScreenResolution = (int)sliderValue;
		
		// Validation
		if (newScreenResolution < 0)
		{
			Debug.LogWarning("Tried to set resolution setting with an invalid value: " + newScreenResolution);
			return;
		}
		
		// Set game screen resolution
		Screen.SetResolution(
			Screen.resolutions[newScreenResolution].width,
			Screen.resolutions[newScreenResolution].height,
			Screen.fullScreenMode // Don't change to fullscreen or windowed mode, just leave it as it is
		);
		
		// Update label text to match new resolution
		UpdateSliderText(newScreenResolution);
	}
	
	/// <summary>
	/// Change slider label text based on Screen.resolutions array index
	/// </summary>
	private void UpdateSliderText(int newScreenResolution)
	{
		// Update slider label
		if (resolutionSettingText)
		{
			// E.g. 1920 x 1080 (60Hz)
			resolutionSettingText.text = Screen.resolutions[newScreenResolution].ToString();
		}
		else
		{
			Debug.LogWarning("Resolution setting text not set in inspector");
		}
	}
}