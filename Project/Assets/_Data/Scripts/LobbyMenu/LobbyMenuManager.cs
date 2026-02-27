using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class LobbyMenuManager : MonoBehaviour
{
	[Header("Settings")]
	// Game can't be started unless at least this many players have joined
	[SerializeField][Range(1,4)] private int minPlayerCount = 1;
	// Incase we ever want to adjust the maximum player count in the future
	[SerializeField][Range(1,4)] private int maxPlayerCount = 4;
	
	[Header("Cache")]
	[Tooltip("References to the Game Object in the scene each gamepad image is on. Index 0 (the first one) will be player 1 and so on.")]
	[SerializeField] private GameObject[] playerControllerImages; // Offset by 1 (0 = player 1)
	[Tooltip("Reference to the Button in the scene that will start the game")]
	[SerializeField] private Button startButton;
	[Tooltip("Reference to the Game Object with the instructions text in the scene")]
	[SerializeField] private GameObject instructionsText;
	
	[Header("Events")]
	[SerializeField] private UnityEvent onPlayerJoined = new UnityEvent();
	
	// The Gamepads that press the join button are added to this list in order.
	// So Gamepad 0 will be player 1 and so on.
	public static List<Gamepad> currentPlayers { get; private set; } = new List<Gamepad>();
	
	#region Lobby
	
	private void Start()
	{
		// Reset list of players whether this is the first time or players quit to main menu then re-entered
		currentPlayers = new List<Gamepad>();
	}
	
	private void Update()
	{
		// Check every frame (as often as possible) if a player is trying to join
		if (currentPlayers.Count < maxPlayerCount)
		{
			PlayerJoinedCheck();
		}
	}
	
	/// <summary>
	/// Check if a player who hasn't already joined presses the join button (X).
	/// If they have call PlayerJoined() passing through the Gamepad object reference
	/// Then check if we have enough players to start the game
	/// </summary>
	private void PlayerJoinedCheck()
	{
		// Check all active gamepads
		// Source - https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Gamepad.html
		foreach (Gamepad gamepad in Gamepad.all)
		{
			// Has this gamepad just pressed the X button?
			if (gamepad.buttonWest.wasPressedThisFrame)
			{
				// Prevent the same player joining twice
				if (!currentPlayers.Contains(gamepad))
					PlayerJoined(gamepad);
			}
		}
		
		// Do we have enough players to start the game?
		if (currentPlayers.Count >= minPlayerCount)
		{
			startButton.interactable = true;
		}
		
		// Have we reached the maximum players?
		if (currentPlayers.Count >= maxPlayerCount)
		{
			instructionsText.SetActive(false);
		}
	}
	
	/// <summary>
	/// This will join a Gamepad (player) to the game by adding them into a list of current players.
	/// It will make sure the same Gamepad isn't being added twice.
	/// After adding it will update the player Gamepad icons.
	/// </summary>
	private void PlayerJoined(Gamepad gamepad)
	{
		// Validation
		if (gamepad == null)
		{
			Debug.LogWarning("Called PlayerJoined without passing in a gamepad");
			return;
		}
		
		// Don't add the same player twice
		if (!currentPlayers.Contains(gamepad))
		{
			// Add the gamepad to the list of current player gamepads.
			// The first Gamepad (index 0) represents player 1 and so on.
			currentPlayers.Add(gamepad);
			
			// Update amount of player gamepad images shown
			UpdatePlayerGamepadUI();
			
			// Let other interested objects know
			onPlayerJoined?.Invoke();
		}
		else
		{
			Debug.LogWarning("Tried to add the same gamepad more than once: " + gamepad);
		}
	}
	
	/// <summary>
	/// Check how many players have joined by seeing how many Gamepads are in the list
	/// Show all the player joined icons in the array up to that point.
	/// For example, if the Gamepad list contains 2 Gamepads (because 2 players have joined), then show the joined icon images index 0 and 1.
	/// </summary>
	private void UpdatePlayerGamepadUI()
	{
		// Loop through all 4 potential player slots
		for (int i = 0; i < maxPlayerCount; i++)
		{
			// Do we have this many players?
			if (currentPlayers.Count > i)
				// Should this player is active
				playerControllerImages[i].SetActive(true);
			else
				// Show this player isn't active
				playerControllerImages[i].SetActive(false);
		}
	}
	
	#endregion Lobby
	
	#region Utility
	
	/// <summary>
	/// Load a scene by specifiing a name. Scene must be added to the Project Build.
	/// This will unload the current scene then load the specified scene (default Unity way).
	/// </summary>
	public void LoadScene(string sceneToLoad)
	{
		// Make sure a scene was specified
		if (sceneToLoad != string.Empty)
			// Load the scene by first unloading the current scene, then loading the new scene (the default Unity way)
			SceneManager.LoadScene(sceneToLoad);
	}
	
	#endregion Utility
}