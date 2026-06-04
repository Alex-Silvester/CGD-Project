using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Change the game's QualitySetting to a value in the QualitySettings array
/// Source - https://docs.unity3d.com/6000.0/Documentation/ScriptReference/QualitySettings.GetQualityLevel.html
/// Source - https://docs.unity3d.com/6000.0/Documentation/ScriptReference/QualitySettings.SetQualityLevel.html
/// Source - https://docs.unity3d.com/6000.0/Documentation/ScriptReference/QualitySettings-names.html
/// </summary>
public class GraphicsQualitySlider : MonoBehaviour
{
	[Header("Cache")]
	[Tooltip("Reference to the UI slider in the scene.")]
    [SerializeField] private Slider slider;
	[Tooltip("Reference to the text in the scene that will display the quality setting (e.g. low, medium, or high).")]
	[SerializeField] private TMP_Text qualitySettingText;
	
    private void Start()
    {
		// Set the slider maximum value to match how quality settings quantity
		// -1 to match array index
		slider.maxValue = QualitySettings.names.Length - 1;
		
        // Setup slider notch position and label text with current graphics quality setting
		// TODO load from save code
        slider.value = QualitySettings.GetQualityLevel();
		SetQualityLevel(slider.value);
    }
	
	/// <summary>
	/// Change the game's QualitySetting to a value in the QualitySettings array
	/// Parameter must be float as slider OnValueChanged only accepts dynamic as a float. The function will convert it to an int before use.
	/// </summary>
	public void SetQualityLevel(float sliderValue)
	{
		// Convert the slider value to the type QualitySetting requires
		int newQualityLevel = (int)sliderValue;
		
		// Validation
		if (newQualityLevel < 0)
		{
			Debug.LogWarning("Tried to set quality level with an invalid value: " + newQualityLevel);
			return;
		}
		
		// Set the game's graphics quality setting
		QualitySettings.SetQualityLevel(newQualityLevel, true);
		
		// Update slider label text with the new setting
		if (qualitySettingText)
			qualitySettingText.text = QualitySettings.names[newQualityLevel];
		else
			Debug.LogWarning("Quality setting text hasn't been set in the inspector");
	}
}