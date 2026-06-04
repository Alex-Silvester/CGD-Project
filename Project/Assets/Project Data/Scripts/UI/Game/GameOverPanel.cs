using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameOverPanel : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Reference to the game over panel game object in the scene. This script can't be placed on the panel itself because it will be deactivated by default (therefore script will never be called)")]
	[SerializeField] private GameObject panel;
	[Tooltip("Reference to the EventSystem in the scene. Needed to set the default button selected by a gamepad")]
	[SerializeField] private EventSystem eventSystem;
	[Tooltip("Reference to the button that should be selected by the gamepad when the game over panel is shown")]
	[SerializeField] private GameObject firstButton;
	
	[Header("Cache")]
	[Tooltip("Reference to the game manager in the scene")]
	[SerializeField] private GameManager gameManager;
	
	private void Start()
	{
		// Subscribe to events
		GameOverState.onEntered += Show;
	}
	
	private void Update()
	{
        // Ignore input checks until game over
        if (!panel.activeSelf)
		{
			return; 
		}
		
		// Check all active gamepads
		// Source - https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Gamepad.html
		foreach (Gamepad gamepad in Gamepad.all)
		{
            // Replay?
            if (gamepad.buttonSouth.wasPressedThisFrame)
			{
				gameManager.ReloadCurrentScene();
			}
			// Main menu?
			else if (gamepad.buttonEast.wasPressedThisFrame)
			{
				gameManager.LoadScene("MainMenu");
			}
		}
	}

	public void Reload()
	{
        gameManager.ReloadCurrentScene();
    }

	public void LoadMenu()
	{
        gameManager.LoadScene("MainMenu");
    }

	/// <summary>
	/// Show the game over panel
	/// Will select the firstButton button
	/// </summary>
	private void Show()
	{
		panel.SetActive(true);

		eventSystem.SetSelectedGameObject(firstButton);
	}
	
	/// <summary>
	/// Hide the game over panel
	/// </summary>
	private void Hide()
	{
		panel.SetActive(false);
	}
	
	// Unsubscribe from GameOverState events as they are static.
	// If we don't the next time the game scene (such as play again) loads GameOverState will try to call both the now destroyed game panel and the new one.
	private void OnDestroy()
	{
		// Unsubscribe to events
		GameOverState.onEntered -= Show;
	}
}