using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Reference to the pause menu panel game object in the scene. This script can't be placed on the panel itself because it will be deactivated by default (therefore script will never be called)")]
	[SerializeField] GameObject pauseMenuPanel;
	
    void Update()
    {
		// Check for pause button input
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) ||
			Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
			TogglePause();
        }
    }
	
	public void TogglePause()
	{
		// Prevent pausing in game states other than playing (e.g. prevent pausing during game over)
		if (GameManager.instance.currentState != GameManager.instance.playingState)
			return;
		
		// Toggle pause status (make opposite of current)
		if (pauseMenuPanel.activeInHierarchy)
			Unpause();
		else
			Pause();
	}
	
	public void Pause()
	{
		// Stop flow of time
		Time.timeScale = 0.0f;
		
		// Show pause menu
		pauseMenuPanel.SetActive(true);
	}
	
	public void Unpause()
	{
		// Set flow of time back to normal
		Time.timeScale = 1.0f;
		
		// Hide pause menu
		pauseMenuPanel.SetActive(false);
	}
}