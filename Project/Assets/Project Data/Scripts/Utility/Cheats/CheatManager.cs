using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Potential cheat codes that could be activated")]
	[SerializeField] private CheatCode[] cheats;
	[Tooltip("Whether we have a valid cheat code won't be checked until we have this amount")]
	[SerializeField] private int requiredInputAmount = 10;
	
	[Header("Sound")]
	[SerializeField] private AudioSource audioSrc;
	[SerializeField] private AudioClip successSound;
	[SerializeField] private AudioClip failureSound;
	
	// List of previous input. Will be stored until "requiredInputAmount" is met.
	private List<string> currentInput = new List<string>();
	
	private void Update()
	{
		if (Gamepad.current == null)
			return;
		
		
		if (Gamepad.current.startButton.isPressed)
		{
			DetectInput();
		}
		else if (Gamepad.current.startButton.wasReleasedThisFrame)
		{
			// Reset any existing inputs
			currentInput = new List<string>();
		}
	}
	
	private void DetectInput()
	{
		// Buttons
        if (Gamepad.current.buttonNorth.wasPressedThisFrame)
            AddInput("Y");
		else if (Gamepad.current.buttonEast.wasPressedThisFrame)
            AddInput("B");
		else if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            AddInput("A");
		else if (Gamepad.current.buttonWest.wasPressedThisFrame)
            AddInput("X");
		
		// Triggers
		else if (Gamepad.current.leftTrigger.wasPressedThisFrame)
            AddInput("LT");
		else if (Gamepad.current.rightTrigger.wasPressedThisFrame)
            AddInput("RT");
		else if (Gamepad.current.leftShoulder.wasPressedThisFrame)
            AddInput("RS");
		else if (Gamepad.current.rightShoulder.wasPressedThisFrame)
            AddInput("LS");
		
		// Dpad
        else if (Gamepad.current.dpad.up.wasPressedThisFrame)
            AddInput("Up");
	    else if (Gamepad.current.dpad.right.wasPressedThisFrame)
            AddInput("Right");
		else if (Gamepad.current.dpad.down.wasPressedThisFrame)
            AddInput("Down");
	    else if (Gamepad.current.dpad.left.wasPressedThisFrame)
            AddInput("Left");
	}
	
	private void AddInput(string input)
	{
		// Add input to list of previous inputs
		currentInput.Add(input);
		
		// Do we have enough inputs to try and activate a cheat?
		if (currentInput.Count >= requiredInputAmount)
		{
			// Look through all cheat codes and try to activate it
			TryToActivateCheat();
			
			// Whatever the outcome, remove previous input and start again
			currentInput = new List<string>();
		}
	}
	
	private bool TryToActivateCheat()
	{
		// Look through all the potential cheats
		foreach (var cheat in cheats)
		{
			// Does the player's input match the cheat's required input?
			for (int i = 0; i < requiredInputAmount; i++)
			{
				// Does it match
				if (currentInput[i] != cheat.GetRequiredInput(i))
				{
					// Stop checking this cheat
					//Debug.Log("Breaking at " + i + ". " + currentInput[i] + " doesn't match " + cheat.GetRequiredInput(i));
					
					// Continue to checking next input
					break;
				}
				
				// Do all inputs match?
				if (i == (requiredInputAmount - 1)) // -1 to match array starting at zero
				{
					// Play confirm sound
					if (audioSrc && successSound)
						audioSrc.PlayOneShot(successSound);
					
					// Activate cheat
					cheat.Activate();
					
					// Show notification
					if (ToastNotificationManager.instance)
						ToastNotificationManager.instance.Add("Cheat Activated", cheat.GetNotificationText());
					
					// Exit function
					return true;
				}
			}
		}
		
		// Play reject sound
		if (audioSrc && failureSound)
			audioSrc.PlayOneShot(failureSound);
		
		// No cheat found
		return false;
	}
}