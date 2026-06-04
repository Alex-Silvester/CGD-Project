using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A Unity UI toggle button that makes the game either fullscreen or windowed.
/// Note: testing only works in a build and doesn't work in the Unity editor
/// Note: Unity automatically saves changes in fullScreenMode to PlayerPrefs - https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Screen.html
/// Source - https://docs.unity3d.com/6000.0/Documentation/ScriptReference/FullScreenMode.html
/// </summary>
public class FullscreenToggle : MonoBehaviour
{
	[Header("Cache")]
	[Tooltip("Reference to Unity UI toggle component in the scene.")]
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        // Set toggle tick starting value to current fullscreen status
		// Start ticked if fullscreen
        toggle.isOn = (Screen.fullScreenMode != FullScreenMode.Windowed);
    }

	/// <summary>
	/// Sets the game to either fullscreen (true) or windowed (false).
	/// Only uses these two to keep it simple for a party game.
	/// </summary>
    public void SetFullscreen(bool status)
    {
		// Set fullscreen or windowed for this session
		if (status)
		{
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
		}
		else
		{
			Screen.fullScreenMode = FullScreenMode.Windowed;
		}
    }
}