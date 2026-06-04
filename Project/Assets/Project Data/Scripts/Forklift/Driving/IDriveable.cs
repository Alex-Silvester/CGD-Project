using UnityEngine;

/// <summary>
/// Use on a game object that should be drivable by the player such as a forklift.
/// Could also be used for other vehicles such as smaller box carry truck.
/// IsVehicleOccupied - check if a player is currently driving the vehicle. Useful for interaction prompts.
/// TryEnterVehicle - attempt to get in a vehicle. Returns false if currently occupied or other reason preventing entry.
/// TryExitVehicle - attempt to get out of vehicle. Returns false if there is no space to get out or other reason.
/// </summary>
public interface IDriveable
{
	public bool IsVehicleOccupied();
	//public bool TryEnterVehicle(PlayerController player); // bool incase vehicle is full
	public bool TryExitVehicle(); // bool incase no position to exit to (e.g. up against wall)
}