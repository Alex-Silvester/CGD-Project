using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameOverPanel : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Reference to the game over panel game object in the scene. This script can't be placed on the panel itself because it will be deactivated by default (therefore script will never be called)")]
	[SerializeField] private GameObject panel;
	[Tooltip("Reference to the EventSystem in the scene. Needed to set the default button selected by a gamepad")]
	[SerializeField] private EventSystem eventSystem;
	[Tooltip("Reference to the button that should be selected by the gamepad when the game over panel is shown")]
	[SerializeField] private GameObject firstButton;
	
	private void Start()
	{
		// Subscribe to events
		GameOverState.onEntered += Show;
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