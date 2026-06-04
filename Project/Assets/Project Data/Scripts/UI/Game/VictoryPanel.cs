using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class VictoryPanel : MonoBehaviour
{
	[Header("Settings")]

	[Tooltip("Reference to the panel game object in the scene. This script can't be placed on the panel itself because it will be deactivated by default (therefore script will never be called)")]
	[SerializeField] private GameObject panel;

	[Tooltip("Reference to the EventSystem in the scene. Needed to set the default button selected by a gamepad")]
	[SerializeField] private EventSystem eventSystem;

	[Tooltip("Reference to the button that should be selected by the gamepad when the game over panel is shown")]
	[SerializeField] private GameObject firstButton;

	[Tooltip("Reference to main score object")]
	[SerializeField] private ScoreObject scoreObject;

	VictoryPanelLayout panelLayout;

    private void Awake()
	{
		if (!panel.TryGetComponent(out panelLayout))
		{
			Debug.Log("Victory panel does not have layout script component.");
		}

        // Subscribe to events
        VictoryState.onEntered += Show;
		VictoryState.onExited += Hide;
	}
	
	/// <summary>
	/// Show the panel
	/// Will select the firstButton button
	/// </summary>
	private void Show()
	{
        panel.SetActive(true);
		panelLayout.UpdateStarIcons(scoreObject);
		eventSystem.SetSelectedGameObject(firstButton);
	}

	
	/// <summary>
	/// Hide the game over panel
	/// </summary>
	private void Hide()
	{
		panelLayout.HideStarIcons();
		panel.SetActive(false);
	}

	// Unsubscribe from VictoryState events as they are static.
	// If we don't the next time the game scene (such as play again) loads VictoryState will try to call both the now destroyed game panel and the new one.
	private void OnDestroy()
	{
		// Unsubscribe to events
		VictoryState.onEntered -= Show;
		VictoryState.onExited -= Hide;
	}
}