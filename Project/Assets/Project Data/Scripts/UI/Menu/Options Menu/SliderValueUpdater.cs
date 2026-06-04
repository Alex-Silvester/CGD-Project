using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Update text when a slider value changes
/// </summary>
public class SliderValueUpdater : MonoBehaviour
{
	[Header("Cache")]
	[Tooltip("Reference to slider component in the scene.")]
    [SerializeField] private Slider slider;
	[Tooltip("Reference to Text Mesh Pro Text in the scene.")]
    [SerializeField] private TMP_Text valueText;

    private void Start()
    {
        // Start the text with the current slider value
        UpdateValueText(slider.value);

		// Update label text when the slider is slid
        slider.onValueChanged.AddListener(UpdateValueText);
    }

    private void UpdateValueText(float value)
    {
		 // Turn slider value into percentage;
        value *= 100;

		// Remove decimal points and add % symbol
		// E.g. 42% instead of 42.1337 to look cleaner
        valueText.text = value.ToString("F0") + "%";
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(UpdateValueText);
    }
}